namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;
    using LogSpect.Formatting.MethodEvents;
    using LogSpect.Logging;

    public sealed class FormattingMethodLoggerFactory : IMethodLoggerFactory
    {
        public FormattingMethodLoggerFactory(ILoggerAdapterFactory loggerAdapterFactory, IIndentationTracker indentationTracker, IMethodEventFormatter methodEventFormatter)
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

            this.LoggerAdapterFactory = loggerAdapterFactory;
            this.IndentationTracker = indentationTracker;
            this.MethodEventFormatter = methodEventFormatter;
        }

        public ILoggerAdapterFactory LoggerAdapterFactory { get; private set; }

        public IIndentationTracker IndentationTracker { get; private set; }

        public IMethodEventFormatter MethodEventFormatter { get; private set; }

        public IMethodLogger Create(MethodBase targetMethod)
        {
            if (targetMethod == null)
            {
                throw new ArgumentNullException("targetMethod");
            }

            ILoggerAdapter adapter = this.LoggerAdapterFactory.Create(targetMethod.DeclaringType);
            object[] attributes = targetMethod.GetCustomAttributes(typeof(LogCallsAttributeBase), false);
            if (attributes.Length == 0)
            {
                throw new ArgumentException("Target method must be decorated with the LogCallsAttribute.");
            }

            LogCallsAttributeBase logCallsAttribute = (LogCallsAttributeBase)attributes[0];
            return new FormattingMethodLogger(targetMethod, logCallsAttribute.Settings, adapter, this.IndentationTracker, this.MethodEventFormatter);
        }
    }
}