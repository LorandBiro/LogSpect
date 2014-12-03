namespace LogSpect.Formatting
{
    using System;
    using System.Text;

    public interface ICustomFormatter
    {
        bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}
