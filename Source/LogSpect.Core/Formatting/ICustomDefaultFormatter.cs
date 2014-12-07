namespace LogSpect.Formatting
{
    using System;
    using System.Text;

    public interface ICustomDefaultFormatter
    {
        bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}
