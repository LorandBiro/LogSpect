namespace LogSpect
{
    using System;
    using System.Globalization;
    using LogSpect.Formatting;
    using LogSpect.Logging;

    public static class LogSpectServiceLocator
    {
        private static IMethodLoggerFactory factoryInstance;

        private static IIndentationService indentationServiceInstance;

        private static ICustomFormatterService customFormatterServiceInstance;

        public static bool IsInitialized { get; private set; }

        public static IMethodLoggerFactory Factory
        {
            get
            {
                EnsureInitialized();
                return factoryInstance;
            }
        }

        public static IIndentationService IndentationService
        {
            get
            {
                EnsureInitialized();
                return indentationServiceInstance;
            }
        }

        public static ICustomFormatterService CustomFormatterService
        {
            get
            {
                EnsureInitialized();
                return customFormatterServiceInstance;
            }
        }

        public static void Initialize(ILoggerAdapterFactory adapterFactory)
        {
            IIndentationService indentationService = new IndentationService(4, 20);
            IFormattingModeReader formattingModeReader = new CachedFormattingModeReader(new FormattingModeReader());
            ICustomFormatterService customFormatterService = new CustomFormatterService();
            IParameterFormatter parameterFormatter = new ParameterFormatter(formattingModeReader, customFormatterService, CultureInfo.InvariantCulture);
            IMethodEventFormatter methodEventFormatter = new MethodEventFormatter(parameterFormatter);
            IMethodLoggerFactory factory = new MethodLoggerFactory(adapterFactory, indentationService, methodEventFormatter);

            Initialize(factory, indentationService);
        }

        public static void Initialize(IMethodLoggerFactory factory, IIndentationService indentationService = null, ICustomFormatterService customFormatterService = null)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("The LogSpect service locator is already initialized.");
            }

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            factoryInstance = factory;
            indentationServiceInstance = indentationService;
            customFormatterServiceInstance = customFormatterService;

            IsInitialized = true;
        }

        private static void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("The LogSpect service locator is not initialized yet.");
            }
        }
    }
}
