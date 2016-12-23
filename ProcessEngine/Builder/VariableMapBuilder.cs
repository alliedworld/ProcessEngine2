namespace KlaudWerk.ProcessEngine.Builder
{
    public class VariableMapBuilder
    {
        private readonly StepBuilder _parent;
        public string VarName { get; private set; }
        public VarRequiredEnum VarRequire { get; private set; }
        public VariableMapBuilder(StepBuilder parent)
        {
            _parent = parent;
            VarRequire=VarRequiredEnum.None;
        }

        public VariableMapBuilder Name(string name)
        {
            VarName = name;
            return this;
        }
        public VariableMapBuilder OnEntry()
        {
            VarRequire|=VarRequiredEnum.OnEntry;
            return this;
        }
        public VariableMapBuilder OnExit()
        {
            VarRequire|=VarRequiredEnum.OnExit;
            return this;
        }
        public VariableMapBuilder None()
        {
            VarRequire=VarRequiredEnum.None;
            return this;
        }
        
        public StepBuilder Remove()
        {
            _parent.RemoveVarMap(this);
            return _parent;
        }
        public StepBuilder Done()
        {
            VarName.NotNull("name").NotEmptyString("name");
            _parent.AddReplaceVarMap(this);
            return _parent;
        }
    }
}