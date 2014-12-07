namespace LogSpect.Log4Net
{
    using System;
    using log4net.Core;
    using LogSpect.Logging;
    using Level = LogSpect.Level;
    using Log4NetLevel = log4net.Core.Level;

    public sealed class Log4NetAdapter : ILoggerAdapter
    {
        private static readonly Log4NetLevel[] Log4NetLevels =
        {
            Log4NetLevel.Trace, Log4NetLevel.Debug, Log4NetLevel.Info, Log4NetLevel.Warn, Log4NetLevel.Error,
            Log4NetLevel.Fatal
        };

        private readonly ILogger logger;

        public Log4NetAdapter(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        public void LogMessage(string message, Level level)
        {
            this.logger.Log(null, ToLog4NetLevel(level), message, null);
        }

        public void LogMessage(string message, Level level, Exception exception)
        {
            this.logger.Log(null, ToLog4NetLevel(level), message, exception);
        }

        public bool IsLevelEnabled(Level level)
        {
            return this.logger.IsEnabledFor(ToLog4NetLevel(level));
        }

        private static Log4NetLevel ToLog4NetLevel(Level level)
        {
            return Log4NetLevels[(int)level];
        }
    }
}
