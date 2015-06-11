namespace LogSpect.Log4Net
{
    using System;
    using log4net.Core;
    using LogSpect.Formatting;

    public sealed class Log4NetAdapterFactory : ILoggerAdapterFactory
    {
        private readonly string loggerName;

        public Log4NetAdapterFactory(string loggerName = null)
        {
            this.loggerName = loggerName;
        }

        public ILoggerAdapter Create(Type targetType)
        {
            ILogger logger = LoggerManager.GetLogger(targetType.Assembly, this.loggerName ?? targetType.FullName);
            return new Log4NetAdapter(logger);
        }
    }
}
