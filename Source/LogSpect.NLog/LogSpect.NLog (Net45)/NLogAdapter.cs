namespace LogSpect.NLog
{
    using System;
    using LogSpect.Formatting;
    using global::NLog;

    public sealed class NLogAdapter : ILoggerAdapter
    {
        private static readonly LogLevel[] LogLevels = { LogLevel.Trace, LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal };

        private readonly Logger logger;

        public NLogAdapter(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        public void LogMessage(string message, Level level)
        {
            this.logger.Log(ToNLogLevel(level), message);
        }

        public void LogMessage(string message, Level level, Exception exception)
        {
            this.logger.Log(ToNLogLevel(level), message, exception);
        }

        public bool IsLevelEnabled(Level level)
        {
            return this.logger.IsEnabled(ToNLogLevel(level));
        }

        private static LogLevel ToNLogLevel(Level level)
        {
            return LogLevels[(int)level];
        }
    }
}
