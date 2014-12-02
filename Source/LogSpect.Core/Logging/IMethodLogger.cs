namespace LogSpect.Logging
{
    using System;
    using System.Reflection;

    public interface IMethodLogger
    {
        MethodBase TargetMethod { get; }

        void LogEnter(object[] parameters);

        void LogLeave(object[] parameters, object returnValue);

        void LogException(Exception exception);
    }
}
