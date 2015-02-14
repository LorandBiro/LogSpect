namespace LogSpect.Logging
{
    using System;
    using System.Linq;
    using System.Reflection;
    using LogSpect.Formatting;

    public sealed class FormattingMethodLogger : IMethodLogger
    {
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

            this.adapter = adapter;
            this.indentationTracker = indentationTracker;
            this.formatter = formatter;

            this.TargetMethod = targetMethod;
            this.Settings = settings;
        }

        public MethodBase TargetMethod { get; private set; }

        public MethodLoggingSettings Settings { get; private set; }

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

            if (this.adapter.IsLevelEnabled(this.Settings.NormalLogLevel))
            {
                string message = this.indentationTracker.Current + this.formatter.SerializeEnter(type, this.TargetMethod, parameters);
                this.adapter.LogMessage(message, this.Settings.NormalLogLevel);
            }

            this.indentationTracker.Increase();
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

            this.indentationTracker.Decrease();

            if (this.adapter.IsLevelEnabled(this.Settings.NormalLogLevel))
            {
                string message = this.indentationTracker.Current + this.formatter.SerializeLeave(type, this.TargetMethod, parameters, returnValue);
                this.adapter.LogMessage(message, this.Settings.NormalLogLevel);
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

            this.indentationTracker.Decrease();

            if (this.adapter.IsLevelEnabled(this.Settings.ExceptionLogLevel))
            {
                bool expected = this.Settings.ExpectedExceptions.Any(x => x.IsInstanceOfType(exception));
                string message = this.indentationTracker.Current + this.formatter.SerializeException(type, this.TargetMethod, exception, expected);
                this.adapter.LogMessage(message, this.Settings.ExceptionLogLevel, exception);
            }
        }
    }
}
