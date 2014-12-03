namespace LogSpect.Formatting
{
    using System;
    using System.Text;

    public interface ICustomFormatterService
    {
        void AddFormatter(ICustomFormatter formatter);

        bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}
