namespace LogSpectRewriter.Rewriting
{
    using System;
    using LogSpect;
    using LogSpectRewriter.Output;
    using Mono.Cecil;

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
                            this.outputWriter.LogWarning(string.Format("{0} doesn't have any effect on interface members.", typeof(LogCallsAttribute).Name), method);
                        }
                    }
                }
            }
        }
    }
}
