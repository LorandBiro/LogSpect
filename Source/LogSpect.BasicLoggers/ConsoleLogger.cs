namespace LogSpect.BasicLoggers
{
    using System;
    using LogSpect.Formatting;

    public sealed class ConsoleLogger : ILoggerAdapter
    {
        public void LogMessage(string message, Level level)
        {
            Console.WriteLine(message);
        }

        public void LogMessage(string message, Level level, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public bool IsLevelEnabled(Level level)
        {
            return true;
        }
    }
}
