namespace LogSpectRewriter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using LogSpectRewriter.Output;
    using LogSpectRewriter.Rewriting;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    // ReSharper disable once UnusedMember.Global
    public class LogSpectRewriterTask : Task
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        [Required]
        public string AssemblyPath { get; set; }

        [Required]
        public string[] References { get; set; }

        public override bool Execute()
        {
            List<string> assemblySearchPaths = this.References.Select(Path.GetDirectoryName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            AssemblyRewriter rewriter = new AssemblyRewriter(new TaskOutputWriter(this.Log));
            return rewriter.TryRewriteAssembly(this.AssemblyPath, this.AssemblyPath, assemblySearchPaths);
        }
    }
}
