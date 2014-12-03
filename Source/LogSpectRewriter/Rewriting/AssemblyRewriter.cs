namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.IO;
    using System.Linq;
    using LogSpect;
    using LogSpectRewriter.Output;
    using Mono.Cecil;
    using Mono.Cecil.Pdb;

    internal sealed class AssemblyRewriter
    {
        private const string LogSpectRewrittenClassName = "<LogSpectRewritten>";

        private readonly IOutputWriter outputWriter;

        public AssemblyRewriter(IOutputWriter outputWriter)
        {
            if (outputWriter == null)
            {
                throw new ArgumentNullException("outputWriter");
            }

            this.outputWriter = outputWriter;
        }

        public bool TryRewriteAssembly(string inputAssemblyPath, string outputAssemblyPath)
        {
            DateTime startedAt = DateTime.UtcNow;
            try
            {
                ModuleDefinition module = LoadModule(inputAssemblyPath);
                if (LogSpectRewrittenClassExists(module))
                {
                    this.outputWriter.LogMessage(string.Format("{0} has been already rewritten.", module.Name));
                    return true;
                }

                this.RewriteModule(module);
                CreateLogSpectRewrittenClass(module);
                SaveModule(module, outputAssemblyPath);
            }
            catch (Exception exception)
            {
                this.outputWriter.LogError("Rewriter failed with unexpected exception.", exception);
                return false;
            }

            this.outputWriter.LogMessage(string.Format("Rewriter completed under {0}.", DateTime.UtcNow - startedAt));
            return true;
        }

        private static ModuleDefinition LoadModule(string assemblyPath)
        {
            string pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
            if (!File.Exists(pdbPath))
            {
                return ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters());
            }

            using (FileStream symbolStream = File.OpenRead(pdbPath))
            {
                ReaderParameters readerParametersWithSymbolStream = new ReaderParameters
                {
                    ReadSymbols = true,
                    SymbolReaderProvider = new PdbReaderProvider(),
                    SymbolStream = symbolStream
                };

                return ModuleDefinition.ReadModule(assemblyPath, readerParametersWithSymbolStream);
            }
        }

        private static void SaveModule(ModuleDefinition module, string assemblyPath)
        {
            module.Write(assemblyPath, new WriterParameters { WriteSymbols = true, SymbolWriterProvider = new PdbWriterProvider() });
        }

        private static bool LogSpectRewrittenClassExists(ModuleDefinition module)
        {
            return module.Types.Any(x => string.IsNullOrEmpty(x.Namespace) && x.Name == LogSpectRewrittenClassName);
        }

        private static void CreateLogSpectRewrittenClass(ModuleDefinition module)
        {
            TypeDefinition logSpectRewritten = new TypeDefinition(string.Empty, LogSpectRewrittenClassName, TypeAttributes.Abstract | TypeAttributes.Sealed, module.TypeSystem.Object);
            module.Types.Add(logSpectRewritten);
        }

        private void RewriteModule(ModuleDefinition module)
        {
            MethodRewriter methodRewriter = new MethodRewriter(module);

            int counter = 0;
            foreach (TypeDefinition typeDefinition in module.Types.Where(typeDefinition => typeDefinition.Methods.Count > 0))
            {
                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                {
                    if (methodDefinition.CustomAttributes.Any(x => x.AttributeType.IsEquivalentTo(typeof(LogCallsAttribute))))
                    {
                        methodRewriter.Rewrite(methodDefinition);
                        counter++;
                    }
                }
            }

            this.outputWriter.LogMessage(string.Format("{0} methods have been rewritten for {1}.", counter, module.Name));
        }
    }
}
