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
            Console.Error.WriteLine(error);
            Console.Error.WriteLine(exception);
        }
    }
}
