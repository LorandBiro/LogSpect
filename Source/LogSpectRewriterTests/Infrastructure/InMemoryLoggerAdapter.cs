﻿namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using System.Text;
    using LogSpect;
    using LogSpect.Logging;

    internal class InMemoryLoggerAdapter : ILoggerAdapter
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        public string Log
        {
            get
            {
                return this.stringBuilder.ToString();
            }
        }

        public void LogMessage(string message, Level level)
        {
            string logEntry = string.Format("{0,7}|{1}", level.ToString().ToUpperInvariant(), message);
            this.stringBuilder.AppendLine(logEntry);
        }

        public void LogMessage(string message, Level level, Exception exception)
        {
            string logEntry = string.Format("{0,7}|{1}", level.ToString().ToUpperInvariant(), message);
            this.stringBuilder.AppendLine(logEntry);
        }

        public bool IsLevelEnabled(Level level)
        {
            return true;
        }

        public void Clear()
        {
            this.stringBuilder.Clear();
        }
    }
}