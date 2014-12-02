namespace LogSpect
{
    using System;
    using System.Globalization;
    using LogSpect.Logging;
    using LogSpect.Serialization;

    public static class LogSpectServiceLocator
    {
        private static IMethodLoggerFactory factoryInstance;

        private static IIndentationService indentationServiceInstance;

        private static ICustomSerializerService customSerializerServiceInstance;

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

        public static ICustomSerializerService CustomSerializerService
        {
            get
            {
                EnsureInitialized();
                return customSerializerServiceInstance;
            }
        }

        public static void Initialize(ILoggerAdapterFactory adapterFactory)
        {
            IIndentationService indentationService = new IndentationService(4, 20);
            ISerializationModeReader serializationModeReader = new CachingSerializationModeReader(new SerializationModeReader());
            ICustomSerializerService customSerializerService = new CustomSerializerService();
            IParameterSerializer parameterSerializer = new ParameterSerializer(serializationModeReader, customSerializerService, CultureInfo.InvariantCulture);
            IMethodEventSerializer methodEventSerializer = new MethodEventSerializer(parameterSerializer);
            IMethodLoggerFactory factory = new MethodLoggerFactory(adapterFactory, indentationService, methodEventSerializer);

            Initialize(factory, indentationService);
        }

        public static void Initialize(IMethodLoggerFactory factory, IIndentationService indentationService = null, ICustomSerializerService customSerializerService = null)
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
            customSerializerServiceInstance = customSerializerService;

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
