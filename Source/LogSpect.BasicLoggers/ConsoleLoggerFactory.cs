﻿namespace LogSpect.BasicLoggers
{
    using System;
    using LogSpect.Formatting;

    public sealed class ConsoleLoggerFactory : ILoggerAdapterFactory
    {
        private readonly ILoggerAdapter logger = new ConsoleLogger();

        public ILoggerAdapter Create(Type targetType)
        {
            return this.logger;
        }
    }
}
