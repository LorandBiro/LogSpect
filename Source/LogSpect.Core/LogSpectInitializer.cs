namespace LogSpect
{
    using System;
    using LogSpect.Logging;

    public static class LogSpectInitializer
    {
        private static IMethodLoggerFactory factoryInstance;

        public static bool IsInitialized
        {
            get
            {
                return factoryInstance != null;
            }
        }

        public static IMethodLoggerFactory Factory
        {
            get
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("LogSpect is not initialized yet.");
                }

                return factoryInstance;
            }
        }

        public static void Initialize(IMethodLoggerFactory factory)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("LogSpect is already initialized.");
            }

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            factoryInstance = factory;
        }
    }
}
