namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using LogSpect.Formatting;

    internal class InMemoryLoggerAdapterFactory : ILoggerAdapterFactory
    {
        public ILoggerAdapter Create(Type targetType)
        {
            return InMemoryLoggerAdapter.Instance;
        }
    }
}
