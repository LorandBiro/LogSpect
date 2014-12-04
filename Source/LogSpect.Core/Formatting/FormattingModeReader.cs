﻿namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;

    public sealed class FormattingModeReader : IFormattingModeReader
    {
        public FormattingMode ReadMode(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            object[] attributes = property.GetCustomAttributes(true);
            return ReadMode(attributes);
        }

        public FormattingMode ReadMode(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            object[] attributes = parameter.GetCustomAttributes(true);
            return ReadMode(attributes);
        }

        private static FormattingMode ReadMode(object[] attributes)
        {
            bool logMembers = false;
            bool logItems = false;

            // ReSharper disable once ForCanBeConvertedToForeach
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
                return logMembers ? FormattingMode.ItemsMembers : FormattingMode.Items;
            }

            return logMembers ? FormattingMode.Members : FormattingMode.Default;
        }
    }
}