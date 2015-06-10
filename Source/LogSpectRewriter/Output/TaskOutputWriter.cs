namespace LogSpectRewriter.Output
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    internal sealed class TaskOutputWriter : IOutputWriter
    {
        private const string Prefix = "LogSpect: ";

        private readonly TaskLoggingHelper log;

        public TaskOutputWriter(TaskLoggingHelper log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            this.log = log;
        }

        public void LogMessage(string message)
        {
            this.log.LogMessage(MessageImportance.High, Prefix + message);
        }

        public void LogError(string error, Exception exception)
        {
            this.log.LogError(Prefix + error);
            this.log.LogErrorFromException(exception, true, true, null);
        }

        public void LogError(string error, Exception exception, string filePath, int lineNumber, int columnNumber)
        {
            this.log.LogError(null, null, null, filePath, lineNumber, columnNumber, 0, 0, Prefix + error);
            this.log.LogErrorFromException(exception, true, true, null);
        }

        public void LogWarning(string warning)
        {
            this.log.LogWarning(Prefix + warning);
        }

        public void LogWarning(string warning, string filePath, int lineNumber, int columnNumber)
        {
            this.log.LogWarning(null, null, null, filePath, lineNumber, columnNumber, 0, 0, Prefix + warning);
        }
    }
}
