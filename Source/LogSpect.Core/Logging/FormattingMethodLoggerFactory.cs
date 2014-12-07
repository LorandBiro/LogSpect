﻿namespace LogSpect.Logging
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using LogSpect.Formatting;

    public sealed class FormattingMethodLoggerFactory : IMethodLoggerFactory
    {
        private readonly ILoggerAdapterFactory adapterFactory;

        private readonly IIndentationTracker indentationTracker;

        private readonly IMethodEventFormatter methodEventFormatter;

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory)
            : this(adapterFactory, null, CultureInfo.InvariantCulture)
        {
        }

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory, IFormatProvider formatProvider)
            : this(adapterFactory, null, formatProvider)
        {
        }

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory, ICustomDefaultFormatter customDefaultFormatter)
            : this(adapterFactory, customDefaultFormatter, CultureInfo.InvariantCulture)
        {
        }

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory, ICustomDefaultFormatter customDefaultFormatter, IFormatProvider formatProvider)
            : this(
                adapterFactory,
                new IndentationTracker(4, 20),
                new MethodEventFormatter(new ParameterFormatter(new FormattingModeReader(), customDefaultFormatter, formatProvider)))
        {
        }

        public FormattingMethodLoggerFactory(ILoggerAdapterFactory adapterFactory, IIndentationTracker indentationTracker, IMethodEventFormatter methodEventFormatter)
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException("adapterFactory");
            }

            if (indentationTracker == null)
            {
                throw new ArgumentNullException("indentationTracker");
            }

            if (methodEventFormatter == null)
            {
                throw new ArgumentNullException("methodEventFormatter");
            }

            this.adapterFactory = adapterFactory;
            this.indentationTracker = indentationTracker;
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
            return new FormattingMethodLogger(targetMethod, logCallsAttribute.Settings, adapter, this.indentationTracker, this.methodEventFormatter);
        }
    }
}