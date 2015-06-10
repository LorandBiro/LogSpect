namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.Linq;
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

            foreach (MethodDefinition method in type.Methods)
            {
                bool hasLogCallsAttribute = false;
                bool hasDoNotLogAttribute = false;
                bool hasLogItemsAttribute = false;
                bool hasLogMembersAttribute = false;
                foreach (CustomAttribute attribute in method.CustomAttributes)
                {
                    if (attribute.AttributeType.IsEquivalentTo(typeof(LogCallsAttribute)))
                    {
                        hasLogCallsAttribute = true;
                    }
                    else if (attribute.AttributeType.IsEquivalentTo(typeof(DoNotLogAttribute)))
                    {
                        hasDoNotLogAttribute = true;
                    }
                    else if (attribute.AttributeType.IsEquivalentTo(typeof(LogItemsAttribute)))
                    {
                        hasLogItemsAttribute = true;
                    }
                    else if (attribute.AttributeType.IsEquivalentTo(typeof(LogMembersAttribute)))
                    {
                        hasLogMembersAttribute = true;
                    }
                }

                if (hasLogCallsAttribute)
                {
                    if (type.IsInterface)
                    {
                        this.outputWriter.LogWarning(string.Format("{0} doesn't have any effect on interface members.", typeof(LogCallsAttribute).Name), method);
                    }
                    else if (type.IsAbstract)
                    {
                        this.outputWriter.LogWarning(string.Format("{0} doesn't have any effect on abstract members.", typeof(LogCallsAttribute).Name), method);
                    }   
                }
                else
                {
                    if (hasDoNotLogAttribute || method.Parameters.Any(x => x.CustomAttributes.Any(y => y.AttributeType.IsEquivalentTo(typeof(DoNotLogAttribute)))))
                    {
                        this.outputWriter.LogWarning(
                            string.Format("{0} doesn't have any effect on methods without {1}.", typeof(DoNotLogAttribute).Name, typeof(LogCallsAttribute).Name),
                            method);
                    }

                    if (hasLogItemsAttribute || method.Parameters.Any(x => x.CustomAttributes.Any(y => y.AttributeType.IsEquivalentTo(typeof(LogItemsAttribute)))))
                    {
                        this.outputWriter.LogWarning(
                            string.Format("{0} doesn't have any effect on methods without {1}.", typeof(LogItemsAttribute).Name, typeof(LogCallsAttribute).Name),
                            method);
                    }

                    if (hasLogMembersAttribute || method.Parameters.Any(x => x.CustomAttributes.Any(y => y.AttributeType.IsEquivalentTo(typeof(LogMembersAttribute)))))
                    {
                        this.outputWriter.LogWarning(
                            string.Format("{0} doesn't have any effect on methods without {1}.", typeof(LogMembersAttribute).Name, typeof(LogCallsAttribute).Name),
                            method);
                    }
                }
            }
        }
    }
}
