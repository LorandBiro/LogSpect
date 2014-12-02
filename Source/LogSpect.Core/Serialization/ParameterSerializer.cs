﻿namespace LogSpect.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public sealed class ParameterSerializer : IParameterSerializer
    {
        private readonly ISerializationModeReader reader;

        private readonly ICustomSerializerService customSerializerService;

        private readonly IFormatProvider formatProvider;

        public ParameterSerializer(ISerializationModeReader reader, ICustomSerializerService customSerializerService, IFormatProvider formatProvider)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (customSerializerService == null)
            {
                throw new ArgumentNullException("customSerializerService");
            }

            if (formatProvider == null)
            {
                throw new ArgumentNullException("formatProvider");
            }

            this.reader = reader;
            this.customSerializerService = customSerializerService;
            this.formatProvider = formatProvider;
        }

        public void Serialize(StringBuilder sb, object value, ParameterInfo parameter)
        {
            if (sb == null)
            {
                throw new ArgumentNullException("sb");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            SerializationMode mode = this.reader.ReadSerializationMode(parameter);
            this.Serialize(sb, value, mode);
        }

        private void SerializeToString(StringBuilder sb, object value)
        {
            if (this.customSerializerService.TrySerialize(sb, value, this.formatProvider))
            {
                return;
            }

            string str = value as string;
            if (str != null)
            {
                sb.Append('"');
                sb.Append(str);
                sb.Append('"');
                return;
            }

            IFormattable formattable = value as IFormattable;
            if (formattable != null)
            {
                sb.Append(formattable.ToString(null, this.formatProvider));
                return;
            }

            sb.Append(value);
        }

        private void Serialize(StringBuilder sb, object value, SerializationMode mode)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }

            switch (mode)
            {
                case SerializationMode.Default:
                    this.SerializeToString(sb, value);
                    break;
                case SerializationMode.Members:
                    this.SerializeMembers(sb, value);
                    break;
                case SerializationMode.Items:
                    this.SerializeItems(sb, value, false);
                    break;
                case SerializationMode.ItemsMembers:
                    this.SerializeItems(sb, value, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", string.Format(CultureInfo.InvariantCulture, "Unexpected serialization mode '{0}'.", mode));
            }
        }

        private void SerializeMembers(StringBuilder sb, object value)
        {
            List<PropertyInfo> properties = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetIndexParameters().Length == 0).ToList();

            sb.Append("{ ");
            for (int i = 0; i < properties.Count; i++)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }

                sb.Append(properties[i].Name);
                sb.Append(": ");

                object memberValue = properties[i].GetValue(value, null);
                SerializationMode mode = this.reader.ReadSerializationMode(properties[i]);
                this.Serialize(sb, memberValue, mode);
            }

            sb.Append(" }");
        }

        private void SerializeItems(StringBuilder sb, object value, bool serializeMembers)
        {
            IDictionary dictionary = value as IDictionary;
            if (dictionary != null)
            {
                bool first = true;
                sb.Append('[');
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    this.SerializeToString(sb, entry.Key);
                    sb.Append(" => ");
                    if (serializeMembers)
                    {
                        this.SerializeMembers(sb, entry.Value);
                    }
                    else
                    {
                        this.SerializeToString(sb, entry.Value);
                    }
                }

                sb.Append(']');
                return;
            }

            ICollection collection = value as ICollection;
            if (collection != null)
            {
                bool first = true;
                sb.Append('[');
                foreach (object item in collection)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    if (serializeMembers)
                    {
                        this.SerializeMembers(sb, item);
                    }
                    else
                    {
                        this.SerializeToString(sb, item);
                    }
                }

                sb.Append(']');
                return;
            }

            if (serializeMembers)
            {
                this.SerializeMembers(sb, value);
            }
            else
            {
                this.SerializeToString(sb, value);
            }
        }
    }
}