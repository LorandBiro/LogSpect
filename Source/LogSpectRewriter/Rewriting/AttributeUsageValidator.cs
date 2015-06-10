namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.Linq;
    using LogSpect;
    using LogSpectRewriter.Output;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public sealed class AttributeUsageValidator
    {
        private readonly IOutputWriter outputWriter;

        public AttributeUsageValidator(IOutputWriter outputWriter)
        {
            if (outputWriter == null)
            {
                throw new ArgumentNullException("outputWriter");
            }

            this.outputWriter = outputWriter;
        }

        public void Validate(TypeDefinition type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsInterface)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    foreach (CustomAttribute attribute in method.CustomAttributes)
                    {
                        if (attribute.AttributeType.IsEquivalentTo(typeof(LogCallsAttribute)))
                        {
                            this.LogWarning(method, string.Format("{0} doesn't have any effect on interface members.", typeof(LogCallsAttribute).Name));
                        }
                    }
                }
            }
        }

        private void LogWarning(MethodDefinition method, string warning)
        {
            warning = string.Format("{0} - {1}", method.FullName, warning);

            if (method.Body == null)
            {
                this.outputWriter.LogWarning(warning);
                return;
            }

            Instruction firstInstructionWithSequencePoint = method.Body.Instructions.FirstOrDefault(x => x.SequencePoint != null);
            if (firstInstructionWithSequencePoint == null)
            {
                this.outputWriter.LogWarning(warning);
                return;
            }

            SequencePoint sequencePoint = firstInstructionWithSequencePoint.SequencePoint;
            this.outputWriter.LogWarning(warning, sequencePoint.Document.Url, sequencePoint.StartLine, sequencePoint.StartColumn);
        }
    }
}
