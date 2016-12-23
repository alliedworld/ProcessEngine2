namespace KlaudWerk.ProcessEngine.Definition
{
    public interface IProcessDefinitionVisitor
    {
        void Visit(ActionDefinition actionDefinition);
        void Visit(LinkDefinition linkDefinition);
        void Visit(StepDefinition stepDefinition);
        void Visit(ScriptDefinition scriptDefinition);
        void Visit(SecurityDefinition securityDefinition);
        void Visit(StepHandlerDefinition stepHandlerDefinition);
        void Visit(VariableDefinition variableDefinition);
        void Visit(VariableMapDefinition variableMapDefinition);
        void Visit(ProcessDefinition processDefinition);
    }

    public interface IProcessDefinitionVisitable
    {
        void Accept(IProcessDefinitionVisitor visitor);
    }
}