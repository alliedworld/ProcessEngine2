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
namespace KlaudWerk.ProcessEngine.Definition
{
    /// <summary>
    /// Visitor Interface for Process Definition
    /// </summary>
    public interface IProcessDefinitionVisitor
    {
        /// <summary>
        /// Visit <see cref="ActionDefinition"/>
        /// </summary>
        /// <param name="actionDefinition"></param>
        void Visit(ActionDefinition actionDefinition);
        /// <summary>
        /// Visit <see cref="LinkDefinition"/>
        /// </summary>
        /// <param name="linkDefinition"></param>
        void Visit(LinkDefinition linkDefinition);
        /// <summary>
        /// Visit <see cref="StepDefinition"/>
        /// </summary>
        /// <param name="stepDefinition"></param>
        void Visit(StepDefinition stepDefinition);
        /// <summary>
        /// Visit <see cref="ScriptDefinition"/>
        /// </summary>
        /// <param name="scriptDefinition"></param>
        void Visit(ScriptDefinition scriptDefinition);
        /// <summary>
        /// Visit <see cref="SecurityDefinition"/>
        /// </summary>
        /// <param name="securityDefinition"></param>
        void Visit(SecurityDefinition securityDefinition);
        /// <summary>
        /// Visit <see cref="StepHandlerDefinition"/>
        /// </summary>
        /// <param name="stepHandlerDefinition"></param>
        void Visit(StepHandlerDefinition stepHandlerDefinition);
        /// <summary>
        /// Visit <see cref="VariableDefinition"/>
        /// </summary>
        /// <param name="variableDefinition"></param>
        void Visit(VariableDefinition variableDefinition);
        /// <summary>
        /// Visit <see cref="ConstraintDefinition"/>
        /// </summary>
        /// <param name="constraintsDefinition"></param>
        void Visit(ConstraintDefinition constraintsDefinition);
        /// <summary>
        /// Visit <see cref="VariableMapDefinition"/>
        /// </summary>
        /// <param name="variableMapDefinition"></param>
        void Visit(VariableMapDefinition variableMapDefinition);
        /// <summary>
        /// Visit <see cref="ActionRelationDefinition"/>
        /// </summary>
        /// <param name="actionRelation"></param>
        void Visit(ActionRelationDefinition actionRelation);
        /// <summary>
        /// Visit <see cref="ProcessDefinition"/>
        /// </summary>
        /// <param name="processDefinition"></param>
        void Visit(ProcessDefinition processDefinition);
    }
}