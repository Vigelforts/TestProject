using System;

namespace BuildSystem
{
    public class BuildSystemException : Exception
    {
        public BuildSystemException(string message)
            : base(message)
        {
            ServiceLocator.Logger.Log(LogLevel.Error, message);
        }

        public BuildSystemException(string message, Exception innerException)
            : base(message)
        {
            ServiceLocator.Logger.Log(LogLevel.Error, string.Format("{0}\n{1}", message, innerException));
        }
    }
}
