namespace LogSpect.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public sealed class MethodLoggingSettings
    {
        public MethodLoggingSettings(Level normalLogLevel, Level exceptionLogLevel, IEnumerable<Type> expectedExceptions)
        {
            if (expectedExceptions == null)
            {
                throw new ArgumentNullException("expectedExceptions");
            }

            this.NormalLogLevel = normalLogLevel;
            this.ExceptionLogLevel = exceptionLogLevel;
            this.ExpectedExceptions = expectedExceptions.ToList();
        }

        public Level NormalLogLevel { get; private set; }

        public Level ExceptionLogLevel { get; private set; }

        public ICollection<Type> ExpectedExceptions { get; private set; }

        public static MethodLoggingSettings GetForMethod(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            object[] attributes = method.GetCustomAttributes(typeof(LogCallsAttributeBase), false);
            if (attributes.Length == 0)
            {
                throw new ArgumentException(string.Format("The method '{0}' must be decorated with the LogCallsAttribute.", method), "method");
            }

            LogCallsAttributeBase logCallsAttribute = (LogCallsAttributeBase)attributes[0];
            return logCallsAttribute.Settings;
        }
    }
}
