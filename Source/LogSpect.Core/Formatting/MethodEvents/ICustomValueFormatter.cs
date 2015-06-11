namespace LogSpect.Formatting.MethodEvents
{
    using System;
    using System.Text;

    public interface ICustomValueFormatter
    {
        bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}
