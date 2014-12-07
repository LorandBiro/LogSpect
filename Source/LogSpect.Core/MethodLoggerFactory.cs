namespace LogSpect
{
    using System;
    using LogSpect.Logging;

    public static class MethodLoggerFactory
    {
        private static IMethodLoggerFactory factoryInstance;

        public static bool IsInitialized
        {
            get
            {
                return factoryInstance != null;
            }
        }

        public static IMethodLoggerFactory Current
        {
            get
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("The LogSpect locator is not initialized yet.");
                }

                return factoryInstance;
            }
        }

        public static void SetFactory(IMethodLoggerFactory factory)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("The LogSpect locator is already initialized.");
            }

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            factoryInstance = factory;
        }
    }
}
