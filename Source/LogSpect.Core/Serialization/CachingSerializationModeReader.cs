namespace LogSpect.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class CachingSerializationModeReader : ISerializationModeReader
    {
        private readonly ISerializationModeReader reader;

        private readonly Dictionary<PropertyInfo, SerializationMode> propertyModes = new Dictionary<PropertyInfo, SerializationMode>();

        private readonly Dictionary<ParameterInfo, SerializationMode> parameterModes = new Dictionary<ParameterInfo, SerializationMode>();

        public CachingSerializationModeReader(ISerializationModeReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.reader = reader;
        }

        public SerializationMode ReadSerializationMode(PropertyInfo property)
        {
            return GetOrAdd(this.propertyModes, property, x => this.reader.ReadSerializationMode(x));
        }

        public SerializationMode ReadSerializationMode(ParameterInfo parameter)
        {
            return GetOrAdd(this.parameterModes, parameter, x => this.reader.ReadSerializationMode(x));
        }

        private static SerializationMode GetOrAdd<TKey>(Dictionary<TKey, SerializationMode> modes, TKey key, Func<TKey, SerializationMode> valueFactory)
        {
            lock (modes)
            {
                SerializationMode value;
                if (!modes.TryGetValue(key, out value))
                {
                    value = valueFactory(key);
                    modes.Add(key, value);
                }

                return value;
            }
        }
    }
}