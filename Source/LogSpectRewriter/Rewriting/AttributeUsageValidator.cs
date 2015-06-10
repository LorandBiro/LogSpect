namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
                        if (method.ReturnType.IsEquivalentTo(typeof(IEnumerable)) || method.ReturnType.IsEquivalentTo(typeof(IEnumerable<>)))
                        {
                            this.outputWriter.LogWarning(
                                string.Format("{0} will work only on ICollection and IDictionary values.", typeof(LogItemsAttribute).Name),
                                method);
                        }
                    }
                    else if (attribute.AttributeType.IsEquivalentTo(typeof(LogMembersAttribute)))
                    {
                        hasLogMembersAttribute = true;
                    }
                }

                bool parameterHasDoNotLogAttribute = false;
                bool parameterHasLogItemsAttribute = false;
                bool parameterHasLogMembersAttribute = false;
                foreach (ParameterDefinition parameter in method.Parameters)
                {
                    foreach (CustomAttribute attribute in parameter.CustomAttributes)
                    {
                        if (attribute.AttributeType.IsEquivalentTo(typeof(DoNotLogAttribute)))
                        {
                            parameterHasDoNotLogAttribute = true;
                        }
                        else if (attribute.AttributeType.IsEquivalentTo(typeof(LogItemsAttribute)))
                        {
                            parameterHasLogItemsAttribute = true;
                            if (parameter.ParameterType.IsEquivalentTo(typeof(IEnumerable)) || parameter.ParameterType.IsEquivalentTo(typeof(IEnumerable<>)))
                            {
                                this.outputWriter.LogWarning(
                                    string.Format("{0} will work only on ICollection and IDictionary values.", typeof(LogItemsAttribute).Name),
                                    method);
                            }
                        }
                        else if (attribute.AttributeType.IsEquivalentTo(typeof(LogMembersAttribute)))
                        {
                            parameterHasLogMembersAttribute = true;
                        }
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
                    if (hasDoNotLogAttribute || parameterHasDoNotLogAttribute)
                    {
                        this.outputWriter.LogWarning(
                            string.Format("{0} doesn't have any effect on methods without {1}.", typeof(DoNotLogAttribute).Name, typeof(LogCallsAttribute).Name),
                            method);
                    }

                    if (hasLogItemsAttribute || parameterHasLogItemsAttribute)
                    {
                        this.outputWriter.LogWarning(
                            string.Format("{0} doesn't have any effect on methods without {1}.", typeof(LogItemsAttribute).Name, typeof(LogCallsAttribute).Name),
                            method);
                    }

                    if (hasLogMembersAttribute || parameterHasLogMembersAttribute)
                    {
                        this.outputWriter.LogWarning(
                            string.Format("{0} doesn't have any effect on methods without {1}.", typeof(LogMembersAttribute).Name, typeof(LogCallsAttribute).Name),
                            method);
                    }
                }
            }

            foreach (PropertyDefinition property in type.Properties)
            {
                if (property.CustomAttributes.Any(x => x.AttributeType.IsEquivalentTo(typeof(LogItemsAttribute)))
                    && (property.PropertyType.IsEquivalentTo(typeof(IEnumerable)) || property.PropertyType.IsEquivalentTo(typeof(IEnumerable<>))))
                {
                    this.outputWriter.LogWarning(
                        string.Format("{0} will work only on ICollection and IDictionary values.", typeof(LogItemsAttribute).Name),
                        property);
                }
            }
        }
    }
}
