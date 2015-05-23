namespace LogSpectRewriter
{
    using System;
    using System.IO;
    using LogSpectRewriter.Output;
    using LogSpectRewriter.Rewriting;

    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("    LogSpectRewriter <inputAssemblyPath>");
                Console.WriteLine("    LogSpectRewriter <inputAssemblyPath> <outputAssemblyPath>");
                Console.WriteLine();
                Console.WriteLine("Possible exit codes:");
                Console.WriteLine("    0 - Rewrite was successful");
                Console.WriteLine("    1 - Invalid arguments");
                Console.WriteLine("    2 - Input file doesn't exist");
                Console.WriteLine("    3 - Unexpected exception");
                return 1;
            }

            string inputAssemblyPath = args[0];
            string outputAssemblyPath = args.Length > 1 ? args[1] : inputAssemblyPath;

            if (!File.Exists(inputAssemblyPath))
            {
                Console.WriteLine("Input file doesn't exist: {0}", inputAssemblyPath);
                return 2;
            }

            AssemblyRewriter rewriter = new AssemblyRewriter(new ConsoleOutputWriter());
            bool success = rewriter.TryRewriteAssembly(inputAssemblyPath, outputAssemblyPath, null);
            return success ? 0 : 3;
        }
    }
}
