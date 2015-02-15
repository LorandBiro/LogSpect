namespace LogSpect
{
    using System;
    using System.Globalization;
    using LogSpect.Formatting;
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

            private set
            {
                if (IsInitialized)
                {
                    throw new InvalidOperationException("LogSpect is already initialized.");
                }

                factoryInstance = value;
            }
        }

        public static void Initialize(IMethodLoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            Factory = factory;
        }

        public static FormattingMethodLoggerFactory Initialize(ILoggerAdapterFactory loggerAdapterFactory)
        {
            return Initialize(loggerAdapterFactory, CultureInfo.InvariantCulture);
        }

        public static FormattingMethodLoggerFactory Initialize(
            ILoggerAdapterFactory loggerAdapterFactory,
            IFormatProvider formatProvider,
            ICustomDefaultFormatter customDefaultFormatter = null)
        {
            if (loggerAdapterFactory == null)
            {
                throw new ArgumentNullException("loggerAdapterFactory");
            }

            if (formatProvider == null)
            {
                throw new ArgumentNullException("formatProvider");
            }

            IFormattingModeReader formattingModeReader = new CachedFormattingModeReader(new FormattingModeReader());
            IMethodEventFormatter methodEventFormatter = new MethodEventFormatter(new ParameterFormatter(formattingModeReader, formatProvider, customDefaultFormatter));

            FormattingMethodLoggerFactory factory = new FormattingMethodLoggerFactory(loggerAdapterFactory, new IndentationTracker(), methodEventFormatter);
            Factory = factory;
            return factory;
        }

        public static FormattingMethodLoggerFactory Initialize(
            ILoggerAdapterFactory loggerAdapterFactory,
            IIndentationTracker indentationTracker,
            IMethodEventFormatter methodEventFormatter)
        {
            if (loggerAdapterFactory == null)
            {
                throw new ArgumentNullException("loggerAdapterFactory");
            }

            if (indentationTracker == null)
            {
                throw new ArgumentNullException("indentationTracker");
            }

            if (methodEventFormatter == null)
            {
                throw new ArgumentNullException("methodEventFormatter");
            }

            FormattingMethodLoggerFactory factory = new FormattingMethodLoggerFactory(loggerAdapterFactory, indentationTracker, methodEventFormatter);
            Factory = factory;
            return factory;
        }
    }
}
