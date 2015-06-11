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

            MethodLoggingSettings settings = MethodLoggingSettings.GetForMethod(targetMethod);
            ILoggerAdapter adapter = this.LoggerAdapterFactory.Create(targetMethod.DeclaringType);
            return new FormattingMethodLogger(targetMethod, settings, adapter, this.IndentationTracker, this.MethodEventFormatter);
        }
    }
}