namespace LogSpect.Formatting
{
    using System;
    using System.Linq;
    using System.Reflection;
    using LogSpect.Formatting.MethodEvents;
    using LogSpect.Logging;

    public sealed class FormattingMethodLogger : IMethodLogger
    {
        private readonly MethodBase targetMethod;

        private readonly MethodLoggingSettings settings;

        private readonly ILoggerAdapter adapter;

        private readonly IIndentationTracker indentationTracker;

        private readonly IMethodEventFormatter formatter;

        public FormattingMethodLogger(
            MethodBase targetMethod,
            MethodLoggingSettings settings,
            ILoggerAdapter adapter,
            IIndentationTracker indentationTracker,
            IMethodEventFormatter formatter)
        {
            if (targetMethod == null)
            {
                throw new ArgumentNullException("targetMethod");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (indentationTracker == null)
            {
                throw new ArgumentNullException("indentationTracker");
            }

            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            this.targetMethod = targetMethod;
            this.settings = settings;
            this.adapter = adapter;
            this.indentationTracker = indentationTracker;
            this.formatter = formatter;
        }

        public void LogEnter(Type type, object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (this.adapter.IsLevelEnabled(this.settings.NormalLogLevel))
            {
                string message = this.indentationTracker.Current + this.formatter.SerializeEnter(type, this.targetMethod, parameters);
                this.adapter.LogMessage(message, this.settings.NormalLogLevel);

                this.indentationTracker.Increase();
            }
        }

        public void LogLeave(Type type, object[] parameters, object returnValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (this.adapter.IsLevelEnabled(this.settings.NormalLogLevel))
            {
                this.indentationTracker.Decrease();

                string message = this.indentationTracker.Current + this.formatter.SerializeLeave(type, this.targetMethod, parameters, returnValue);
                this.adapter.LogMessage(message, this.settings.NormalLogLevel);
            }
        }

        public void LogException(Type type, Exception exception)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            if (this.adapter.IsLevelEnabled(this.settings.ExceptionLogLevel))
            {
                this.indentationTracker.Decrease();

                bool expected = this.settings.ExpectedExceptions.Any(x => x.IsInstanceOfType(exception));
                string message = this.indentationTracker.Current + this.formatter.SerializeException(type, this.targetMethod, exception, expected);
                this.adapter.LogMessage(message, this.settings.ExceptionLogLevel, exception);
            }
        }
    }
}
