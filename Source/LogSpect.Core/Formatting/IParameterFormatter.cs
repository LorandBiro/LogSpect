namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;
    using System.Text;

    public interface IParameterFormatter
    {
        void Serialize(StringBuilder sb, object value, MemberInfo member);

        void Serialize(StringBuilder sb, object value, ParameterInfo parameter);
    }
}
