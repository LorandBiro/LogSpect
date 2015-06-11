namespace LogSpect.BasicLoggers
{
    using System;
    using LogSpect.Formatting;

    public sealed class ColoredConsoleLoggerFactory : ILoggerAdapterFactory
    {
        private readonly ColoredConsoleLogger logger = new ColoredConsoleLogger();

        public ILoggerAdapter Create(Type targetType)
        {
            return this.logger;
        }
    }
}