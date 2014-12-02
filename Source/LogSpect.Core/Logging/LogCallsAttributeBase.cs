namespace LogSpect.Logging
{
    using System;

    public abstract class LogCallsAttributeBase : Attribute
    {
        protected LogCallsAttributeBase(Level normalLogLevel, Level exceptionLogLevel, params Type[] expectedExceptions)
        {
            this.Settings = new MethodLoggingSettings(normalLogLevel, exceptionLogLevel, expectedExceptions ?? new Type[0]);
        }

        public MethodLoggingSettings Settings { get; private set; }
    }
}
