namespace LogSpect
{
    using System;
    using System.Globalization;
    using LogSpect.Formatting;
    using LogSpect.Formatting.MethodEvents;
    using LogSpect.Logging;

    public static class LogSpectInitializer
    {
        static LogSpectInitializer()
        {
            Factory = DummyMethodLoggerFactory.Instance;
        }

        public static bool IsInitialized { get; private set; }

        public static IMethodLoggerFactory Factory { get; private set; }

        public static void Initialize(IMethodLoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            Factory = factory;
            IsInitialized = true;
        }

        public static FormattingMethodLoggerFactory Initialize(ILoggerAdapterFactory loggerAdapterFactory)
        {
            return Initialize(loggerAdapterFactory, CultureInfo.InvariantCulture);
        }

        public static FormattingMethodLoggerFactory Initialize(
            ILoggerAdapterFactory loggerAdapterFactory,
            IFormatProvider formatProvider,
            ICustomValueFormatter customValueFormatter = null)
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
            IMethodEventFormatter methodEventFormatter = new MethodEventFormatter(new ParameterFormatter(formattingModeReader, formatProvider, customValueFormatter));

            FormattingMethodLoggerFactory factory = new FormattingMethodLoggerFactory(loggerAdapterFactory, new IndentationTracker(), methodEventFormatter);
            Initialize(factory);
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
            Initialize(factory);
            return factory;
        }
    }
}
