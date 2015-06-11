namespace LogSpect.BasicLoggers
{
    using System;
    using System.IO;
    using LogSpect.Formatting;

    public sealed class TextFileLogger : ILoggerAdapter
    {
        private readonly string logFilePath;

        public TextFileLogger(string logFilePath)
        {
            if (logFilePath == null)
            {
                throw new ArgumentNullException("logFilePath");
            }

            this.logFilePath = logFilePath;
        }

        public void LogMessage(string message, Level level)
        {
            string logEntry = string.Format("{0,7}|{1} - {2}{3}", level, DateTime.Now, message, Environment.NewLine);
            File.AppendAllText(this.logFilePath, logEntry);
        }

        public void LogMessage(string message, Level level, Exception exception)
        {
            string logEntry = string.Format("{0,7}|{1} - {2}{3}{4}{3}", level, DateTime.Now, message, Environment.NewLine, exception);
            File.AppendAllText(this.logFilePath, logEntry);
        }

        public bool IsLevelEnabled(Level level)
        {
            return true;
        }
    }
}
