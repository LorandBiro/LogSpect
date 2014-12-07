namespace LogSpect.NLog
{
    using System;
    using LogSpect.Logging;
    using global::NLog;

    public sealed class NLogAdapterFactory : ILoggerAdapterFactory
    {
        private readonly string loggerName;

        public NLogAdapterFactory(string loggerName = null)
        {
            this.loggerName = loggerName;
        }

        public ILoggerAdapter Create(Type targetType)
        {
            // LogManager is thread-safe and the loggers are cached.
            Logger logger = LogManager.GetLogger(this.loggerName ?? targetType.FullName);
            return new NLogAdapter(logger);
        }
    }
}
