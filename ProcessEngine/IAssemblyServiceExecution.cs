using System;

namespace KlaudWerk.ProcessEngine
{
    public interface IAssemblyServiceExecution
    {
        bool TryLoadClass(string fullClassName,out IExecutionService service, out Exception error);
    }
}