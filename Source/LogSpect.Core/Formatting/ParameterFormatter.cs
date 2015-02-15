namespace LogSpect.Formatting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public sealed class ParameterFormatter : IParameterFormatter
    {
        private readonly IFormattingModeReader reader;

        private readonly IFormatProvider formatProvider;

        private readonly ICustomDefaultFormatter customDefaultFormatter;

        public ParameterFormatter(IFormattingModeReader reader, IFormatProvider formatProvider, ICustomDefaultFormatter customDefaultFormatter = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (formatProvider == null)
            {
                throw new ArgumentNullException("formatProvider");
            }

            this.reader = reader;
            this.formatProvider = formatProvider;
            this.customDefaultFormatter = customDefaultFormatter;
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

            FormattingMode mode = this.reader.ReadMode(parameter);
            this.Serialize(sb, value, mode);
        }

        private void SerializeDefault(StringBuilder sb, object value)
        {
            if (this.customDefaultFormatter != null && this.customDefaultFormatter.TrySerialize(sb, value, this.formatProvider))
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

        private void Serialize(StringBuilder sb, object value, FormattingMode mode)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }

            switch (mode)
            {
                case FormattingMode.Default:
                    this.SerializeDefault(sb, value);
                    break;
                case FormattingMode.Members:
                    this.SerializeMembers(sb, value);
                    break;
                case FormattingMode.Items:
                    this.SerializeItems(sb, value, false);
                    break;
                case FormattingMode.ItemsMembers:
                    this.SerializeItems(sb, value, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", string.Format(CultureInfo.InvariantCulture, "Unexpected formatting mode '{0}'.", mode));
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
                FormattingMode mode = this.reader.ReadMode(properties[i]);
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

                    this.SerializeDefault(sb, entry.Key);
                    sb.Append(" => ");
                    if (serializeMembers)
                    {
                        this.SerializeMembers(sb, entry.Value);
                    }
                    else
                    {
                        this.SerializeDefault(sb, entry.Value);
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
                        this.SerializeDefault(sb, item);
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
                this.SerializeDefault(sb, value);
            }
        }
    }
}