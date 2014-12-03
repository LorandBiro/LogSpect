namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;

    public interface IMethodEventFormatter
    {
        string SerializeEnter(MethodBase method, object[] parameters);

        string SerializeLeave(MethodBase method, object[] parameters, object returnValue);

        string SerializeException(MethodBase method, Exception exception, bool expected);
    }
}
