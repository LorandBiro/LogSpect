namespace LogSpect.Logging
{
    using System;
    using System.Reflection;

    public interface IMethodLoggerFactory
    {
        IMethodLogger Create(MethodBase targetMethod);
    }
}
