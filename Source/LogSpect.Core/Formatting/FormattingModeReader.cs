namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;

    public sealed class FormattingModeReader : IFormattingModeReader
    {
        public FormattingMode ReadMode(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            object[] attributes = member.GetCustomAttributes(true);
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
                else if (attributes[i] is DoNotLogAttribute)
                {
                    return FormattingMode.DoNotLog;
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