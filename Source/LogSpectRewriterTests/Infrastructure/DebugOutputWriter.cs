﻿namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using System.Diagnostics;
    using LogSpectRewriter.Output;

    internal class DebugOutputWriter : IOutputWriter
    {
        public void LogMessage(string message)
        {
            Debug.WriteLine("LogSpect: " + message);
        }

        public void LogError(string error, Exception exception)
        {
            Debug.WriteLine("LogSpect: " + error);
            Debug.WriteLine(exception.ToString());
        }

        public void LogWarning(string warning)
        {
            Debug.WriteLine("LogSpect: " + warning);
        }

        public void LogWarning(string warning, string filePath, int lineNumber, int columnNumber)
        {
            Debug.WriteLine("LogSpect: " + warning);
        }
    }
}