namespace LogSpect
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    public sealed class LogItemsAttribute : Attribute
    {
    }
}
