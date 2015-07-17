namespace LogSpect.Logging
{
    using System;

    public sealed class DummyMethodLogger : IMethodLogger
    {
        public static readonly DummyMethodLogger Instance = new DummyMethodLogger();

        private DummyMethodLogger()
        {
        }

        public void LogEnter(Type type, object[] parameters)
        {
        }

        public void LogLeave(Type type, object[] parameters, object returnValue)
        {
        }

        public void LogException(Type type, Exception exception)
        {
        }
    }
}
