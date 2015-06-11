namespace LogSpect.Formatting
{
    using System;

    public interface ILoggerAdapterFactory
    {
        ILoggerAdapter Create(Type targetType);
    }
}
