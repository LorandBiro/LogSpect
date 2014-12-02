namespace LogSpect.Serialization
{
    using System;
    using System.Text;

    public interface ICustomSerializer
    {
        bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}
