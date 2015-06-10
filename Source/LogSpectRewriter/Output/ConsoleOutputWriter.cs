namespace LogSpectRewriter.Output
{
    using System;

    internal sealed class ConsoleOutputWriter : IOutputWriter
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string error, Exception exception)
        {
            Console.Error.WriteLine("error : {0}", error);
            Console.Error.WriteLine(exception);
        }

        public void LogError(string error, Exception exception, string filePath, int lineNumber, int columnNumber)
        {
            Console.Error.WriteLine("{0}({1},{2},{3},{4}): error : {5}", filePath, lineNumber, columnNumber, 0, 0, error);
            Console.Error.WriteLine(exception);
        }

        public void LogWarning(string warning)
        {
            Console.WriteLine("warning : {0}", warning);
        }

        public void LogWarning(string warning, string filePath, int lineNumber, int columnNumber)
        {
            Console.WriteLine("{0}({1},{2},{3},{4}): warning : {5}", filePath, lineNumber, columnNumber, 0, 0, warning);
        }
    }
}
