namespace LogSpect.BasicLoggers
{
    using System;
    using LogSpect.Formatting;

    public sealed class ColoredConsoleLogger : ILoggerAdapter
    {
        public void LogMessage(string message, Level level)
        {
            WriteColored(message, level);
        }

        public void LogMessage(string message, Level level, Exception exception)
        {
            WriteColored(message + Environment.NewLine + exception, level);
        }

        public bool IsLevelEnabled(Level level)
        {
            return true;
        }

        private static void WriteColored(string message, Level level)
        {
            ConsoleColor previousForeground = Console.ForegroundColor;
            ConsoleColor previousBackground = Console.BackgroundColor;

            switch (level)
            {
                case Level.Trace:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case Level.Debug:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case Level.Info:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case Level.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case Level.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case Level.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }

            Console.WriteLine(message);

            Console.ForegroundColor = previousForeground;
            Console.BackgroundColor = previousBackground;
        }
    }
}
