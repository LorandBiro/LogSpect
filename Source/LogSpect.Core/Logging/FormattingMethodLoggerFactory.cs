namespace LogSpect.Logging
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using LogSpect.Formatting;

    public sealed class FormattingMethodLoggerFactory : IMethodLoggerFactory
    {
        private readonly ILoggerAdapterFactory adapterFactory;

        private readonly IIndentationService indentationService;

        private readonly IMethodEventFormatter methodEventFormatter;

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory)
            : this(
                adapterFactory,
                new IndentationService(4, 20),
                new MethodEventFormatter(new ParameterFormatter(new FormattingModeReader(), new CustomFormatterService(), CultureInfo.InvariantCulture)))
        {
        }

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory, IIndentationService indentationService, IMethodEventFormatter methodEventFormatter)
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException("adapterFactory");
            }

            if (indentationService == null)
            {
                throw new ArgumentNullException("indentationService");
            }

            if (methodEventFormatter == null)
            {
                throw new ArgumentNullException("methodEventFormatter");
            }

            this.adapterFactory = adapterFactory;
            this.indentationService = indentationService;
            this.methodEventFormatter = methodEventFormatter;
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
            return new FormattingMethodLogger(targetMethod, logCallsAttribute.Settings, adapter, this.indentationService, this.methodEventFormatter);
        }
    }
}