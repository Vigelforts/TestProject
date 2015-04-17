using System;

namespace BuildSystem
{
    public sealed class ConsoleLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            Console.WriteLine(string.Format("{0}: {1}", level, message));
        }
    }
}
