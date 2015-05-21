namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using LogSpect;
    using LogSpect.Logging;
    using LogSpectRewriter.Rewriting;
    using Microsoft.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal static class CodeRunner
    {
        private const string TempDirectoryPath = "Temp";

        private const string TestClassName = "RewriterTest";

        private const string TestMethodName = "Run";

        private static readonly AssemblyRewriter Rewriter = new AssemblyRewriter(new DebugOutputWriter());

        private static bool isInitialized;

        public static void CompileRewriteAndRun(string classDefinitions, string testCode, string expectedOutput)
        {
            EnsureInitialized();

            string assemblyFilePath = CompileAssemblyFromSource(classDefinitions, testCode);
            RewriteAssembly(assemblyFilePath);

            InMemoryLoggerAdapterFactory.Adapter.Clear();
            LoadAssemblyAndRunTest(assemblyFilePath);
            Assert.AreEqual(expectedOutput, InMemoryLoggerAdapterFactory.Adapter.Log);
        }

        private static void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            LogSpectInitializer.Initialize(new InMemoryLoggerAdapterFactory());

            if (!Directory.Exists(TempDirectoryPath))
            {
                Directory.CreateDirectory(TempDirectoryPath);
                return;
            }

            foreach (string filePath in Directory.GetFiles(TempDirectoryPath))
            {
                File.Delete(filePath);
            }

            isInitialized = true;
        }

        private static string CompileAssemblyFromSource(string classDefinitions, string testCode)
        {
            string source = string.Format("using LogSpect; using Microsoft.VisualStudio.TestTools.UnitTesting; {0} public static class {1} {{ public static void {2}() {{ {3} }} }}", classDefinitions, TestClassName, TestMethodName, testCode);

            string outputPath = Path.Combine(TempDirectoryPath, Guid.NewGuid() + ".dll");

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters options = new CompilerParameters { GenerateExecutable = false, OutputAssembly = outputPath };
            options.ReferencedAssemblies.Add(typeof(LogCallsAttribute).Assembly.ManifestModule.FullyQualifiedName);
            options.ReferencedAssemblies.Add(typeof(LogCallsAttributeBase).Assembly.ManifestModule.FullyQualifiedName);
            options.ReferencedAssemblies.Add(typeof(Assert).Assembly.ManifestModule.FullyQualifiedName);

            CompilerResults results = provider.CompileAssemblyFromSource(options, source);

            foreach (CompilerError error in results.Errors)
            {
                Debug.WriteLine(error);
            }

            Assert.IsFalse(results.Errors.HasErrors, "Failed to compile the test code.");

            return outputPath;
        }

        private static void RewriteAssembly(string assemblyFilePath)
        {
            bool success = Rewriter.TryRewriteAssembly(assemblyFilePath, assemblyFilePath);
            Assert.IsTrue(success, "Failed to rewrite the test subject assembly.");
        }

        private static void LoadAssemblyAndRunTest(string filePath)
        {
            byte[] assembly = File.ReadAllBytes(filePath);
            MethodInfo testMethod = Assembly.Load(assembly).GetType(TestClassName).GetMethod(TestMethodName);
            testMethod.Invoke(null, null);
        }
    }
}
