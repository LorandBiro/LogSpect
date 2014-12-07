namespace LogSpect.Logging
{
    using System;
    using System.Linq;
    using System.Reflection;
    using LogSpect.Formatting;

    public sealed class FormattingMethodLogger : IMethodLogger
    {
        private readonly ILoggerAdapter adapter;

        private readonly IIndentationService indentationService;

        private readonly IMethodEventFormatter formatter;

        public FormattingMethodLogger(MethodBase targetMethod, MethodLoggingSettings settings, ILoggerAdapter adapter, IIndentationService indentationService, IMethodEventFormatter formatter)
        {
            if (targetMethod == null)
            {
                throw new ArgumentNullException("targetMethod");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (indentationService == null)
            {
                throw new ArgumentNullException("indentationService");
            }

            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            
            this.adapter = adapter;
            this.indentationService = indentationService;
            this.formatter = formatter;

            this.TargetMethod = targetMethod;
            this.Settings = settings;
        }

        public MethodBase TargetMethod { get; private set; }

        public MethodLoggingSettings Settings { get; private set; }

        public void LogEnter(object[] parameters)
        {
            if (this.adapter.IsLevelEnabled(this.Settings.NormalLogLevel))
            {
                string message = this.indentationService.Current + this.formatter.SerializeEnter(this.TargetMethod, parameters);
                this.adapter.LogMessage(message, this.Settings.NormalLogLevel);
            }

            this.indentationService.Increase();
        }

        public void LogLeave(object[] parameters, object returnValue)
        {
            this.indentationService.Decrease();

            if (this.adapter.IsLevelEnabled(this.Settings.NormalLogLevel))
            {
                string message = this.indentationService.Current + this.formatter.SerializeLeave(this.TargetMethod, parameters, returnValue);
                this.adapter.LogMessage(message, this.Settings.NormalLogLevel);
            }
        }

        public void LogException(Exception exception)
        {
            this.indentationService.Decrease();

            if (this.adapter.IsLevelEnabled(this.Settings.ExceptionLogLevel))
            {
                bool expected = this.Settings.ExpectedExceptions.Any(x => x.IsInstanceOfType(exception));
                string message = this.indentationService.Current + this.formatter.SerializeException(this.TargetMethod, exception, expected);
                this.adapter.LogMessage(message, this.Settings.ExceptionLogLevel, exception);
            }
        }
    }
}
