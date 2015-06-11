namespace LogSpect.Formatting.MethodEvents
{
    using System;
    using System.Reflection;

    public interface IMethodEventFormatter
    {
        string SerializeEnter(Type type, MethodBase method, object[] parameters);

        string SerializeLeave(Type type, MethodBase method, object[] parameters, object returnValue);

        string SerializeException(Type type, MethodBase method, Exception exception, bool expected);
    }
}
