namespace LogSpect.Logging
{
    using System;
    using System.Reflection;

    public interface IMethodLogger
    {
        MethodBase TargetMethod { get; }

        void LogEnter(Type type, object[] parameters);

        void LogLeave(Type type, object[] parameters, object returnValue);

        void LogException(Type type, Exception exception);
    }
}
