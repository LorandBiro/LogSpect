namespace LogSpect.Logging
{
    using System;

    public interface IMethodLogger
    {
        void LogEnter(Type type, object[] parameters);

        void LogLeave(Type type, object[] parameters, object returnValue);

        void LogException(Type type, Exception exception);
    }
}
