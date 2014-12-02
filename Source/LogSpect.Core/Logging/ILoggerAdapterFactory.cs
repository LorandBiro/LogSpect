namespace LogSpect.Logging
{
    using System;

    public interface ILoggerAdapterFactory
    {
        ILoggerAdapter Create(Type targetType);
    }
}
