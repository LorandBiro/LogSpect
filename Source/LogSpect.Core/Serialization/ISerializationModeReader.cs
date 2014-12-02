namespace LogSpect.Serialization
{
    using System;
    using System.Reflection;

    public interface ISerializationModeReader
    {
        /// <summary>
        /// Reads the attributes of the specified property and determines how it should be serialized when the containing object is logged.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>The mode of serialization.</returns>
        /// <exception cref="ArgumentNullException">If property is null.</exception>
        SerializationMode ReadSerializationMode(PropertyInfo property);

        SerializationMode ReadSerializationMode(ParameterInfo parameter);
    }
}
