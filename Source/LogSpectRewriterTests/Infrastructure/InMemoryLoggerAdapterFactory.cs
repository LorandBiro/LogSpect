namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using LogSpect.Logging;

    internal class InMemoryLoggerAdapterFactory : ILoggerAdapterFactory
    {
        public ILoggerAdapter Create(Type targetType)
        {
            return InMemoryLoggerAdapter.Instance;
        }
    }
}
