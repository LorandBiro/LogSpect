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
            SequencePoint sequencePoint = GetSequencePoint(method.Body);

            if (sequencePoint == null)
            {
                outputWriter.LogWarning(warning);
                return;
            }

            outputWriter.LogWarning(warning, sequencePoint.Document.Url, sequencePoint.StartLine, sequencePoint.StartColumn);
        }

        public static void LogWarning(this IOutputWriter outputWriter, string warning, PropertyDefinition property)
        {
            warning = string.Format("{0} - {1}", property.FullName, warning);
            SequencePoint sequencePoint = GetSequencePoint(property.GetMethod.Body) ?? GetSequencePoint(property.SetMethod.Body);

            if (sequencePoint == null)
            {
                outputWriter.LogWarning(warning);
                return;
            }

            outputWriter.LogWarning(warning, sequencePoint.Document.Url, sequencePoint.StartLine, sequencePoint.StartColumn);
        }

        public static void LogError(this IOutputWriter outputWriter, string error, Exception exception, MethodDefinition method)
        {
            error = string.Format("{0} - {1}", method.FullName, error);
            SequencePoint sequencePoint = GetSequencePoint(method.Body);

            if (sequencePoint == null)
            {
                outputWriter.LogError(error, exception);
                return;
            }

            outputWriter.LogError(error, exception, sequencePoint.Document.Url, sequencePoint.StartLine, sequencePoint.StartColumn);
        }

        private static SequencePoint GetSequencePoint(MethodBody body)
        {
            if (body == null)
            {
                return null;
            }

            Instruction instruction = body.Instructions.FirstOrDefault(x => x.SequencePoint != null);
            return instruction == null ? null : instruction.SequencePoint;
        }
    }
}
