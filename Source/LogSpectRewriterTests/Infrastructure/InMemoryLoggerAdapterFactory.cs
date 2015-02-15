namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using LogSpect.Logging;

    internal class InMemoryLoggerAdapterFactory : ILoggerAdapterFactory
    {
        public static readonly InMemoryLoggerAdapter Adapter = new InMemoryLoggerAdapter();

        public ILoggerAdapter Create(Type targetType)
        {
            return Adapter;
        }
    }
}
