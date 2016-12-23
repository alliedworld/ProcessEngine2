using System;

namespace KlaudWerk.ProcessEngine.Runtime
{
    [Serializable]
    public class InvalidProcessStateException : Exception
    {
        public InvalidProcessStateException()
        {
        }

        public InvalidProcessStateException(string message) : base(message)
        {
        }
    }
}