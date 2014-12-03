namespace LogSpect.Logging
{
    using System;

    public abstract class LogCallsAttributeBase : Attribute
    {
        private static readonly Type[] EmptyTypes = new Type[0];

        protected LogCallsAttributeBase(Level normalLogLevel, Level exceptionLogLevel, params Type[] expectedExceptions)
        {
            this.Settings = new MethodLoggingSettings(normalLogLevel, exceptionLogLevel, expectedExceptions ?? EmptyTypes);
        }

        public MethodLoggingSettings Settings { get; private set; }
    }
}
