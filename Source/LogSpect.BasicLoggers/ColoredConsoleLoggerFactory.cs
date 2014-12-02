namespace LogSpect.BasicLoggers
{
    using System;
    using LogSpect.Logging;

    public sealed class ColoredConsoleLoggerFactory : ILoggerAdapterFactory
    {
        private readonly ColoredConsoleLogger logger = new ColoredConsoleLogger();

        public ILoggerAdapter Create(Type targetType)
        {
            return this.logger;
        }
    }
}