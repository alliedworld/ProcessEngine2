using System;
using System.Collections.Generic;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Default tag service provider
    /// </summary>
    public class TagHandlerProvider : ITagServiceProvider
    {
        private readonly Func<string,ITageService> _iocTagServiceProvider;

        public TagHandlerProvider(Func<string,ITageService> iocTagServiceProvider)
        {
            _iocTagServiceProvider = iocTagServiceProvider;
        }

        public ITageService GetTagService(TagDefinition tagDefinition)
        {
            switch (tagDefinition.Handler.StepHandlerType)
            {
                case StepHandlerTypeEnum.IoC:
                    return new IocTagService(tagDefinition.Id, _iocTagServiceProvider);
                case StepHandlerTypeEnum.Script:
                    return new ScriptTagService(tagDefinition);
                case StepHandlerTypeEnum.Service:
                case StepHandlerTypeEnum.None:
                    return new EmptyTagService(tagDefinition.Id, tagDefinition.DisplayName);
                default:
                    throw new ArgumentException($"Invalid or unsupported Step Handler type:{tagDefinition.Handler.StepHandlerType}");
            }
        }
        /// <summary>
        /// Empty Service does not produce anything
        /// </summary>
        private class EmptyTagService : ITageService
        {
            private static readonly IReadOnlyList<string> _values=new List<string>();
            public string Name { get; }
            public string DisplayName { get; }

            public IReadOnlyList<string> GetValues(IProcessRuntimeEnvironment env)
            {
                return _values;
            }



            public EmptyTagService(string name, string displayName)
            {
                Name = name;
                DisplayName = displayName;

            }
        }
        /// <summary>
        /// Tag Service that use IOC
        /// </summary>
        private class IocTagService : ITageService
        {
            public string Name { get; }
            public string DisplayName { get; }
            private readonly ITageService _service;

            /// <summary>
            /// IoC Service constructor
            /// </summary>
            public IocTagService(string name,Func<string,ITageService> iocTagServiceProvider)
            {
                _service = iocTagServiceProvider(name);
                Name = _service.Name;
                DisplayName = _service.DisplayName;
            }
            public IReadOnlyList<string> GetValues(IProcessRuntimeEnvironment env)
            {
                return _service.GetValues(env);
            }

        }
        /// <summary>
        /// Script service
        /// </summary>
        private class ScriptTagService : ITageService
        {
            private CsScriptRuntimeGeneric<string[]> _sc;
            private bool _compiled;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="tagDefinition"></param>
            public ScriptTagService(TagDefinition tagDefinition)
            {
                Name = tagDefinition.Id;
                DisplayName = tagDefinition.DisplayName;
                _sc=new CsScriptRuntimeGeneric<string[]>(tagDefinition.Handler.Script);
            }

            public string Name { get; }
            public string DisplayName { get; }

            public IReadOnlyList<string> GetValues(IProcessRuntimeEnvironment env)
            {
                if (!_compiled)
                {
                    string[] errors;
                    if(!_sc.TryCompile(out errors))
                        throw new ArgumentException(
                            string.Format("Compiletion error for Tag {0} : {1}",Name,string.Join(";",errors)));
                    _compiled = true;
                }
                return _sc.Execute(env).Result;
            }
        }
    }
}