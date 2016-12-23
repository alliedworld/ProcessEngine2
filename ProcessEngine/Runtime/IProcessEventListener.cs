namespace KlaudWerk.ProcessEngine.Runtime
{
    /// <summary>
    /// Process event listener
    /// </summary>
    public interface IProcessEventListener
    {
        void OnStartProcess();
        void OnProcessError();
        void OnProcessFinished();
        void BeforeStepEnter();
        void OnStepEnter();
        void BeforeStepExit();
        void OnStepExit();
        void BeforeProcessSuspended();
        void OnProcessSuspended();
        void BeforeProcessResumed();
        void OnProcessResumed();
    }
}