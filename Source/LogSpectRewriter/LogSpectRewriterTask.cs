namespace LogSpectRewriter
{
    using System;
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

        public override bool Execute()
        {
            AssemblyRewriter rewriter = new AssemblyRewriter(new TaskOutputWriter(this.Log));
            return rewriter.TryRewriteAssembly(this.AssemblyPath, this.AssemblyPath);
        }
    }
}
