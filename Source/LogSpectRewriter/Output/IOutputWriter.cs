namespace LogSpectRewriter.Output
{
    using System;

    public interface IOutputWriter
    {
        void LogMessage(string message);

        void LogError(string error, Exception exception);
    }
}
