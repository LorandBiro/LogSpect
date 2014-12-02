namespace LogSpect.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
    }
}
