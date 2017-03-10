using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Runtime class for Variable Definitions
    /// </summary>
    public class VariableRuntime: ScriptCompilableBase
    {
        private readonly VariableDefinition _vd;

        /// <summary>
        /// Variable Definition
        /// </summary>
        public VariableDefinition Definition => _vd;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vd">Variable definition</param>
        public VariableRuntime(VariableDefinition vd)
        {
            _vd = vd;
        }
        /// <summary>
        /// Try to compile the script
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public override bool TryCompile(out string[] errors)
        {
            if (_vd.HandlerDefinition == null)
            {
                errors = new string[] { };
                return true;
            }
            return TryCompileScript(_vd.HandlerDefinition.Script, out errors);
        }
    }
}