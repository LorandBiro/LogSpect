namespace LogSpect.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class CustomSerializerService : ICustomSerializerService
    {
        private readonly List<ICustomSerializer> serializers = new List<ICustomSerializer>();

        public void AddSerializer(ICustomSerializer customSerializer)
        {
            if (customSerializer == null)
            {
                throw new ArgumentNullException("customSerializer");
            }

            this.serializers.Add(customSerializer);
        }

        public bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < this.serializers.Count; i++)
            {
                if (this.serializers[i].TrySerialize(sb, value, formatProvider))
                {
                    return true;
                }
            }

            return false;
        }
    }
}