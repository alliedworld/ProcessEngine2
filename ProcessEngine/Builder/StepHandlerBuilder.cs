namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Builds the step handler
    /// </summary>
    public class StepHandlerBuilder
    {
        private readonly StepBuilder _parent;
        public ScriptBuilder<StepHandlerBuilder> ScriptBuilder { get; private set; }
        public string IocName { get; private set; }
        public string FullClassName { get; private set; }
        public StepHandlerTypeEnum StepHandlerType { get; private set; }

        public StepHandlerBuilder(StepBuilder parent)
        {
            _parent = parent;
            StepHandlerType=StepHandlerTypeEnum.None;
        }

        public StepHandlerBuilder IocService(string name)
        {
            IocName = name;
            FullClassName = string.Empty;
            StepHandlerType=StepHandlerTypeEnum.IoC;
            return this;
        }

        public StepHandlerBuilder HumanTask()
        {
            IocName = string.Empty;
            FullClassName = string.Empty;
            StepHandlerType=StepHandlerTypeEnum.Task;
            return this;
        }

        public StepHandlerBuilder Service(string fullClassName)
        {
            IocName = string.Empty;
            FullClassName = string.Empty;
            StepHandlerType=StepHandlerTypeEnum.Service;
            return this;
        }

        public ScriptBuilder<StepHandlerBuilder> Script()
        {
            if(ScriptBuilder==null)
                ScriptBuilder=new ScriptBuilder<StepHandlerBuilder>(this);
            StepHandlerType=StepHandlerTypeEnum.Script;
            return ScriptBuilder;
        }

        public StepBuilder Done()
        {
            return _parent;
        }
    }
}