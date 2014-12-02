namespace LogSpectRewriter.Output
{
    using System;

    internal interface IOutputWriter
    {
        void LogMessage(string message);

        void LogError(string error, Exception exception);
    }
}
