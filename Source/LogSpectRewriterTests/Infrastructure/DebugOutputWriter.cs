namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using LogSpectRewriter.Output;

    internal class DebugOutputWriter : IOutputWriter
    {
        public static readonly DebugOutputWriter Instance = new DebugOutputWriter();

        private readonly StringBuilder warnings = new StringBuilder();

        private DebugOutputWriter()
        {
        }

        public string Warnings
        {
            get
            {
                return this.warnings.ToString();
            }
        }

        public void LogMessage(string message)
        {
            Debug.WriteLine("LogSpect: " + message);
        }

        public void LogError(string error, Exception exception)
        {
            Debug.WriteLine("LogSpect Error: " + error);
            Debug.WriteLine(exception.ToString());
        }

        public void LogError(string error, Exception exception, string filePath, int lineNumber, int columnNumber)
        {
            Debug.WriteLine("LogSpect Error: " + error);
            Debug.WriteLine(exception.ToString());
        }

        public void LogWarning(string warning)
        {
            this.warnings.AppendLine(warning);
            Debug.WriteLine("LogSpect Warning: " + warning);
        }

        public void LogWarning(string warning, string filePath, int lineNumber, int columnNumber)
        {
            this.warnings.AppendLine(warning);
            Debug.WriteLine("LogSpect Warning: " + warning);
        }

        public void Clear()
        {
            this.warnings.Clear();
        }
    }
}