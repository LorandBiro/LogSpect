namespace LogSpectRewriter.Output
{
    using System;
    using System.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public static class OutputWriterExtensions
    {
        public static void LogWarning(this IOutputWriter outputWriter, string warning, MethodDefinition method)
        {
            warning = string.Format("{0} - {1}", method.FullName, warning);

            if (method.Body == null)
            {
                outputWriter.LogWarning(warning);
                return;
            }

            Instruction firstInstructionWithSequencePoint = method.Body.Instructions.FirstOrDefault(x => x.SequencePoint != null);
            if (firstInstructionWithSequencePoint == null)
            {
                outputWriter.LogWarning(warning);
                return;
            }

            SequencePoint sequencePoint = firstInstructionWithSequencePoint.SequencePoint;
            outputWriter.LogWarning(warning, sequencePoint.Document.Url, sequencePoint.StartLine, sequencePoint.StartColumn);
        }

        public static void LogError(this IOutputWriter outputWriter, string error, Exception exception, MethodDefinition method)
        {
            error = string.Format("{0} - {1}", method.FullName, error);

            if (method.Body == null)
            {
                outputWriter.LogError(error, exception);
                return;
            }

            Instruction firstInstructionWithSequencePoint = method.Body.Instructions.FirstOrDefault(x => x.SequencePoint != null);
            if (firstInstructionWithSequencePoint == null)
            {
                outputWriter.LogError(error, exception);
                return;
            }

            SequencePoint sequencePoint = firstInstructionWithSequencePoint.SequencePoint;
            outputWriter.LogError(error, exception, sequencePoint.Document.Url, sequencePoint.StartLine, sequencePoint.StartColumn);
        }
    }
}
