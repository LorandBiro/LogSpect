namespace LogSpect.Serialization
{
    using System;

    /// <summary>
    /// The serialization mode of a property, a parameter or a return value.
    /// </summary>
    public enum SerializationMode
    {
        Default,
        Members,
        Items,
        ItemsMembers,
    }
}
