namespace LogSpectRewriter.Output
{
    using System;

    public interface IOutputWriter
    {
        void LogMessage(string message);

        void LogError(string error, Exception exception);

        void LogWarning(string warning);

        void LogWarning(string warning, string filePath, int lineNumber, int columnNumber);
    }
}
