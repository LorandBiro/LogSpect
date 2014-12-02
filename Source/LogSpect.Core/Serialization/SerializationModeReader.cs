namespace LogSpect.Serialization
{
    using System;
    using System.Reflection;

    public sealed class SerializationModeReader : ISerializationModeReader
    {
        public SerializationMode ReadSerializationMode(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            object[] attributes = property.GetCustomAttributes(true);
            return ReadSerializationMode(attributes);
        }

        public SerializationMode ReadSerializationMode(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            object[] attributes = parameter.GetCustomAttributes(true);
            return ReadSerializationMode(attributes);
        }

        private static SerializationMode ReadSerializationMode(object[] attributes)
        {
            bool logMembers = false;
            bool logItems = false;
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is LogMembersAttribute)
                {
                    logMembers = true;
                }
                else if (attributes[i] is LogItemsAttribute)
                {
                    logItems = true;
                }
            }

            if (logItems)
            {
                return logMembers ? SerializationMode.ItemsMembers : SerializationMode.Items;
            }

            return logMembers ? SerializationMode.Members : SerializationMode.Default;
        }
    }
}