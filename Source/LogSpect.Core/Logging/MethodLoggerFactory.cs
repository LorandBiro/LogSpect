namespace LogSpect.Logging
{
    using System;
    using System.Reflection;
    using LogSpect.Serialization;

    public sealed class MethodLoggerFactory : IMethodLoggerFactory
    {
        private readonly ILoggerAdapterFactory adapterFactory;

        private readonly IIndentationService indentationService;

        private readonly IMethodEventSerializer methodEventSerializer;

        public MethodLoggerFactory(ILoggerAdapterFactory adapterFactory, IIndentationService indentationService, IMethodEventSerializer methodEventSerializer)
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException("adapterFactory");
            }

            if (indentationService == null)
            {
                throw new ArgumentNullException("indentationService");
            }

            if (methodEventSerializer == null)
            {
                throw new ArgumentNullException("methodEventSerializer");
            }

            this.adapterFactory = adapterFactory;
            this.indentationService = indentationService;
            this.methodEventSerializer = methodEventSerializer;
        }

        public IMethodLogger Create(MethodBase targetMethod)
        {
            if (targetMethod == null)
            {
                throw new ArgumentNullException("targetMethod");
            }

            ILoggerAdapter adapter = this.adapterFactory.Create(targetMethod.DeclaringType);
            object[] attributes = targetMethod.GetCustomAttributes(typeof(LogCallsAttributeBase), false);
            if (attributes.Length == 0)
            {
                throw new ArgumentException("Target method must have decorated with the LogCallsAttribute.");
            }

            LogCallsAttributeBase logCallsAttribute = (LogCallsAttributeBase)attributes[0];
            return new MethodLogger(targetMethod, logCallsAttribute.Settings, adapter, this.indentationService, this.methodEventSerializer);
        }
    }
}