/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Fclp;
using KlaudWerk.ProcessEngine.Definition;
using KlaudWerk.ProcessEngine.Persistence;

namespace Klaudwerk.ProcessDeployer
{
    public class ApplicationArgumments
    {
        public string Command { get; set; }
        public string FlowId { get; set; }
        public int Version { get; set; }

        public override string ToString()
        {
            return $"{Command} : id={FlowId} version={Version}";
        }
    }
    internal class Program
    {

        private static readonly Deployer WfDeployer=new Deployer();
        private static readonly Dictionary<string,Action<ApplicationArgumments>>
            Commands=new Dictionary<string, Action<ApplicationArgumments>>
            {
                {"help",PrintHelp},
                {"h",PrintHelp},
                {"?",PrintHelp},
                {"la",ListWorkflowasInAssemblies},
                {"listassemblies",ListWorkflowasInAssemblies},
                {"ld",ListdeployedWorkflows},
                {"listdeployed",ListdeployedWorkflows},
                {"d",DeployWorkflow},
                {"deploy",DeployWorkflow},
                {"u",UndeployWorkflow},
                {"undeploy",UndeployWorkflow},
                {"a",ActivateWorkflow},
                {"activate",ActivateWorkflow},
                {"x",DeactivateWorkflow},
                {"deactivate",DeactivateWorkflow},
            };

        public static void PrintHelp(ApplicationArgumments a)
        {
            Console.WriteLine("use -c <command> [-f <flowguid>] [-v <version>]");
            string commands = @"
commands:
 h,? or help          - print help
 la or listassemblies - list workflows in assemblies catalog
 ld or listdeployed   - list deployed workflowa
 d  or deploy         - deploy a workflow
 u  or undeploy       - undeploy a workflow
 a  or activate       - activate workflow
 x  or deactivate     - deactivate workflow
";
          Console.WriteLine(commands);
        }

        public static void ListWorkflowasInAssemblies(ApplicationArgumments a)
        {
            Console.WriteLine("ID                                  |MD5                     |Name - Description");
            Console.WriteLine("---------------------------------------------------------------------------------");
            WfDeployer.Processes.ToList().ForEach(Console.WriteLine);
        }

        public static void ListdeployedWorkflows(ApplicationArgumments a)
        {
            Console.WriteLine("ID                                  |MD5                     |Version|Status|Name - Description");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");

            WfDeployer.DeployedWorkflows.ToList().ForEach(w =>
            {
                string str = $"{w.Id}|{w.Md5}|{w.Version}|{w.Status}|{w.Name}-{w.Description} {w.LastUpdated}";
                Console.WriteLine(str);
            });
        }

        public static void DeployWorkflow(ApplicationArgumments a)
        {
            Guid guid = CheckId(a);
            if (guid == Guid.Empty)
            {
                Console.WriteLine("Valid Process Definition GUID required.");
                return;
            }
            try
            {
                WfDeployer.DeployWorkflow(guid);
                Console.WriteLine($"Process ID={guid} has been deployed.");
                ListdeployedWorkflows(a);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deploying process ID={guid}. Error={ex.Message}");
            }


        }

        public static void UndeployWorkflow(ApplicationArgumments a)
        {
            DeactivateWorkflow(a);
        }

        public static void ActivateWorkflow(ApplicationArgumments a)
        {
            SetWorkflowStatius(a,ProcessDefStatusEnum.Active);
            ListdeployedWorkflows(a);
        }

        public static void DeactivateWorkflow(ApplicationArgumments a)
        {
            SetWorkflowStatius(a, ProcessDefStatusEnum.NotActive);
            ListdeployedWorkflows(a);
        }

        private static void SetWorkflowStatius(ApplicationArgumments a, ProcessDefStatusEnum status)
        {
            Guid guid = CheckId(a);
            if (guid == Guid.Empty)
            {
                Console.WriteLine("Valid Process Definition GUID required.");
                return;
            }
            if (a.Version == -1)
            {
                Console.WriteLine("Version number is required.");
                return;
            }
            WfDeployer.SetWorkflowStatus(guid, a.Version, status);
        }

        private static Guid CheckId(ApplicationArgumments a)
        {
            if (string.IsNullOrEmpty(a.FlowId))
            {
                return Guid.Empty;
            }
            Guid guid;
            return Guid.TryParse(a.FlowId, out guid) ? guid : Guid.Empty;
        }

        public static void Main(string[] args)
        {
            // Initialize the composition catalog
            var aggregationPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "Workflows");
            if (Directory.Exists(aggregationPath))
            {
                var directoryCatalog = new DirectoryCatalog(aggregationPath);
                using (CompositionContainer container = new CompositionContainer(directoryCatalog))
                {
                    IEnumerable<IProcessDefinitionRegistry> registries =
                        container.GetExportedValues<IProcessDefinitionRegistry>();
                    WfDeployer.Registries = registries;
                }
            }

            var parser = new FluentCommandLineParser<ApplicationArgumments>();
            parser.Setup(a => a.Command).As('c', "command").Required().SetDefault("help");
            parser.Setup(a => a.FlowId).As('f', "flowid").SetDefault("");
            parser.Setup(a => a.Version).As('v', "version").SetDefault(-1);
            var result = parser.Parse(args);
            if (!result.HasErrors)
            {
                Action<ApplicationArgumments> action;
                if (!Commands.TryGetValue(parser.Object.Command, out action))
                {
                    PrintHelp(parser.Object);
                    return;
                }
                action(parser.Object);
            }
            else
            {
                Console.WriteLine(result.ErrorText);
                Console.WriteLine("Use -c help for help");
            }
        }

    }
}