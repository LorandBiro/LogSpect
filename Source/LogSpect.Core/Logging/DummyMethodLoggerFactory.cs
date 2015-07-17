namespace LogSpect.Logging
{
    using System;
    using System.Reflection;

    public sealed class DummyMethodLoggerFactory : IMethodLoggerFactory
    {
        public static readonly DummyMethodLoggerFactory Instance = new DummyMethodLoggerFactory();

        private DummyMethodLoggerFactory()
        {
        }

        public IMethodLogger Create(MethodBase targetMethod)
        {
            return DummyMethodLogger.Instance;
        }
    }
}
