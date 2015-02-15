namespace LogSpectRewriterTests.Infrastructure
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using Microsoft.Win32.SafeHandles;

    internal static class FileHelper
    {
        public static bool CanBeDeleted(string filePath)
        {
            using (SafeFileHandle fileHandle = CreateFile(filePath, FileSystemRights.Modify, FileShare.Write, IntPtr.Zero, FileMode.OpenOrCreate, FileOptions.None, IntPtr.Zero))
            {
                return !fileHandle.IsInvalid;
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            FileSystemRights dwDesiredAccess,
            FileShare dwShareMode,
            IntPtr securityAttrs,
            FileMode dwCreationDisposition,
            FileOptions dwFlagsAndAttributes,
            IntPtr hTemplateFile);
    }
}
