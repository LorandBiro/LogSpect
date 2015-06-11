namespace LogSpect.Formatting
{
    using System;

    public interface ILoggerAdapter
    {
        void LogMessage(string message, Level level);

        void LogMessage(string message, Level level, Exception exception);

        bool IsLevelEnabled(Level level);
    }
}
