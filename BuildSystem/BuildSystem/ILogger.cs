using System;

namespace BuildSystem
{
    public enum LogLevel
    {
        Info, Warning, Error
    }

    public interface ILogger
    {
        void Log(LogLevel level, string message);
    }
}
