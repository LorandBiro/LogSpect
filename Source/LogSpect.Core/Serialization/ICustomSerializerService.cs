namespace LogSpect.Serialization
{
    using System;
    using System.Text;

    public interface ICustomSerializerService
    {
        void AddSerializer(ICustomSerializer customSerializer);

        bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}
