namespace LogSpect.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class CustomFormatterService : ICustomFormatterService
    {
        private readonly List<ICustomFormatter> formatters = new List<ICustomFormatter>();

        public void AddFormatter(ICustomFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            this.formatters.Add(formatter);
        }

        public bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < this.formatters.Count; i++)
            {
                if (this.formatters[i].TrySerialize(sb, value, formatProvider))
                {
                    return true;
                }
            }

            return false;
        }
    }
}