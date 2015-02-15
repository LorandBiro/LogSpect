namespace LogSpectRewriterTests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using LogSpect;
    using LogSpect.Logging;
    using LogSpectRewriter.Rewriting;
    using LogSpectRewriterTests.Infrastructure;
    using Microsoft.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal static class CodeRunner
    {
        private const string TempDirectoryPath = "GeneratedAssemblies";

        public static void CompileRewriteAndRun(string source, string expectedOutput)
        {
            InitializeTempDirectory();
            string filePath = CompileAssembly(source);
            RewriteAssembly(filePath);
            InitializeLogSpect();
            LoadAndRunAssembly(filePath);
            Assert.AreEqual(expectedOutput, InMemoryLoggerAdapterFactory.Adapter.Log);
        }

        private static string CompileAssembly(string source)
        {
            Debug.WriteLine("= Compiling source code");

            string outputPath = Path.Combine(TempDirectoryPath, Guid.NewGuid() + ".dll");
            Debug.WriteLine(string.Format("Output path: {0}", outputPath));

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters options = new CompilerParameters { GenerateExecutable = false, OutputAssembly = outputPath };
            options.ReferencedAssemblies.Add(typeof(LogCallsAttribute).Assembly.ManifestModule.Name);
            options.ReferencedAssemblies.Add(typeof(LogCallsAttributeBase).Assembly.ManifestModule.Name);

            CompilerResults results = provider.CompileAssemblyFromSource(options, source);

            foreach (CompilerError error in results.Errors)
            {
                Debug.WriteLine(error);
            }

            Assert.IsFalse(results.Errors.HasErrors, "Failed to compile the test code.");
            Debug.WriteLine("Assembly successfully compiled.");

            return outputPath;
        }

        private static void InitializeTempDirectory()
        {
            if (!Directory.Exists(TempDirectoryPath))
            {
                Directory.CreateDirectory(TempDirectoryPath);
                return;
            }

            foreach (string filePath in Directory.GetFiles(TempDirectoryPath))
            {
                if (FileHelper.CanBeDeleted(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private static void RewriteAssembly(string filePath)
        {
            Debug.WriteLine("= Rewriting assembly");

            AssemblyRewriter rewriter = new AssemblyRewriter(new DebugOutputWriter());
            bool success = rewriter.TryRewriteAssembly(filePath, filePath);

            Assert.IsTrue(success, "Failed to rewrite the test subject assembly.");
            Debug.WriteLine("Assembly rewrite completed successfully.");
        }

        private static void InitializeLogSpect()
        {
            if (!LogSpectInitializer.IsInitialized)
            {
                LogSpectInitializer.Initialize(new InMemoryLoggerAdapterFactory());
            }

            InMemoryLoggerAdapterFactory.Adapter.Clear();
        }

        private static void LoadAndRunAssembly(string filePath)
        {
            Debug.WriteLine("= Loading and running code");

            Assembly assembly = Assembly.LoadFrom(filePath);
            List<MethodInfo> publicStaticMethods =
                assembly.GetTypes()
                    .Where(x => x.IsPublic)
                    .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    .Where(x => x.GetParameters().Length == 0)
                    .ToList();

            Assert.IsTrue(publicStaticMethods.Count == 1, "The code must contain exactly 1 public static parameterless method to run.");

            MethodInfo method = publicStaticMethods.Single();
            method.Invoke(null, null);
        }
    }
}
