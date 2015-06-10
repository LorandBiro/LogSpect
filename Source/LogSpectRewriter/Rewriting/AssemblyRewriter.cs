namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using LogSpect;
    using LogSpectRewriter.Output;
    using Mono.Cecil;
    using Mono.Cecil.Pdb;

    public sealed class AssemblyRewriter
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

        public bool TryRewriteAssembly(string inputAssemblyPath, string outputAssemblyPath, ICollection<string> assemblySearchPaths)
        {
            if (inputAssemblyPath == null)
            {
                throw new ArgumentNullException("inputAssemblyPath");
            }

            if (outputAssemblyPath == null)
            {
                throw new ArgumentNullException("outputAssemblyPath");
            }

            this.outputWriter.LogMessage(string.Format("Rewriter started on '{0}'.", inputAssemblyPath));
            DateTime startedAt = DateTime.UtcNow;
            bool success;

            try
            {
                ModuleDefinition module = LoadModule(inputAssemblyPath, assemblySearchPaths);
                if (LogSpectRewrittenClassExists(module))
                {
                    success = true;
                    this.outputWriter.LogMessage(string.Format("Assembly has been already rewritten."));
                }
                else
                {
                    success = this.RewriteModule(module);
                    if (success)
                    {
                        CreateLogSpectRewrittenClass(module);
                        SaveModule(module, outputAssemblyPath);
                    }
                }
            }
            catch (Exception exception)
            {
                success = false;
                this.outputWriter.LogError("Rewriter failed with unexpected exception.", exception);
            }

            this.outputWriter.LogMessage(string.Format("Rewriter completed under {0}.", DateTime.UtcNow - startedAt));
            return success;
        }

        private static ModuleDefinition LoadModule(string assemblyPath, ICollection<string> assemblySearchPaths)
        {
            DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
            if (assemblySearchPaths == null)
            {
                assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));   
            }
            else
            {
                foreach (string assemblySearchPath in assemblySearchPaths)
                {
                    assemblyResolver.AddSearchDirectory(assemblySearchPath);
                }
            }

            string pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
            if (!File.Exists(pdbPath))
            {
                return ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { AssemblyResolver = assemblyResolver });
            }

            using (FileStream symbolStream = File.OpenRead(pdbPath))
            {
                ReaderParameters readerParametersWithSymbolStream = new ReaderParameters
                {
                    ReadSymbols = true,
                    SymbolReaderProvider = new PdbReaderProvider(),
                    SymbolStream = symbolStream,
                    AssemblyResolver = assemblyResolver
                };

                return ModuleDefinition.ReadModule(assemblyPath, readerParametersWithSymbolStream);
            }
        }

        private static void SaveModule(ModuleDefinition module, string assemblyPath)
        {
            if (module.HasSymbols)
            {
                module.Write(assemblyPath, new WriterParameters { WriteSymbols = true, SymbolWriterProvider = new PdbWriterProvider() });   
            }
            else
            {
                module.Write(assemblyPath);
            }
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

        private bool RewriteModule(ModuleDefinition module)
        {
            MethodRewriter methodRewriter = new MethodRewriter(module);
            AttributeUsageValidator validator = new AttributeUsageValidator(this.outputWriter);

            int counter = 0;
            bool success = true;
            foreach (TypeDefinition typeDefinition in module.Types)
            {
                validator.Validate(typeDefinition);

                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                {
                    if (methodDefinition.CustomAttributes.Any(x => x.AttributeType.IsEquivalentTo(typeof(LogCallsAttribute))))
                    {
                        try
                        {
                            methodRewriter.Rewrite(methodDefinition);
                            counter++;
                        }
                        catch (Exception exception)
                        {
                            success = false;
                            this.outputWriter.LogError("Rewriting the method failed with unexpected exception.", exception, methodDefinition);
                        }
                    }
                }
            }

            this.outputWriter.LogMessage(string.Format("{0} methods have been rewritten.", counter));
            return success;
        }
    }
}
