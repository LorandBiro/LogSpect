namespace LogSpect.BasicLoggers
{
    using System;
    using LogSpect.Logging;

    public sealed class TextFileLoggerFactory : ILoggerAdapterFactory
    {
        private readonly TextFileLogger logger;

        public TextFileLoggerFactory(string logFilePath)
        {
            if (logFilePath == null)
            {
                throw new ArgumentNullException("logFilePath");
            }

            this.logger = new TextFileLogger(logFilePath);
        }

        public ILoggerAdapter Create(Type targetType)
        {
            return this.logger;
        }
    }
}
