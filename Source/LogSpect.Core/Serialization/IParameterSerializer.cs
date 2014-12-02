namespace LogSpect.Serialization
{
    using System;
    using System.Reflection;
    using System.Text;

    public interface IParameterSerializer
    {
        void Serialize(StringBuilder sb, object value, ParameterInfo parameter);
    }
}
