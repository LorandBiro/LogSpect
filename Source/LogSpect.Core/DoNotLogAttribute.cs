namespace LogSpect
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property)]
    public sealed class DoNotLogAttribute : Attribute
    {
    }
}
