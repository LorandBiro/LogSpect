namespace LogSpect
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class DoNotLogAttribute : Attribute
    {
    }
}
