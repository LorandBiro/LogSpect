namespace LogSpect.Serialization
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    public sealed class MethodEventSerializer : IMethodEventSerializer
    {
        private readonly IParameterSerializer parameterSerializer;

        public MethodEventSerializer(IParameterSerializer parameterSerializer)
        {
            if (parameterSerializer == null)
            {
                throw new ArgumentNullException("parameterSerializer");
            }

            this.parameterSerializer = parameterSerializer;
        }

        public string SerializeEnter(MethodBase method, object[] parameters)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            Debug.Assert(method.DeclaringType != null, "Global methods are not supported.");

            StringBuilder sb = new StringBuilder();
            sb.Append("Enter ");
            sb.Append(method.DeclaringType.Name);
            sb.Append(".");
            sb.Append(method.Name);
            sb.Append("(");
            this.AppendParameters(sb, method, parameters, false);
            sb.Append(")");

            return sb.ToString();
        }

        public string SerializeLeave(MethodBase method, object[] parameters, object returnValue)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            Debug.Assert(method.DeclaringType != null, "Global methods are not supported.");

            StringBuilder sb = new StringBuilder();
            sb.Append("Leave ");
            sb.Append(method.DeclaringType.Name);
            sb.Append(".");
            sb.Append(method.Name);
            sb.Append("(");
            this.AppendParameters(sb, method, parameters, true);
            sb.Append(")");

            if (!method.IsConstructor)
            {
                MethodInfo methodInfo = (MethodInfo)method;
                if (methodInfo.ReturnType != typeof(void))
                {
                    sb.Append(": ");
                    this.parameterSerializer.Serialize(sb, returnValue, methodInfo.ReturnParameter);
                }
            }

            return sb.ToString();
        }

        public string SerializeException(MethodBase method, Exception exception, bool expected)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            Debug.Assert(method.DeclaringType != null, "Global methods are not supported.");

            StringBuilder sb = new StringBuilder();
            if (expected)
            {
                sb.Append(method.DeclaringType.Name);
                sb.Append(".");
                sb.Append(method.Name);
                sb.Append("() threw an exception ");
                sb.Append(exception.GetType().Name);
                sb.Append(". Message: ");
                sb.Append(exception.Message);
            }
            else
            {
                sb.Append(method.DeclaringType.Name);
                sb.Append(".");
                sb.Append(method.Name);
                sb.Append("() failed with ");
                sb.Append(exception.GetType().Name);
                sb.Append("! Message: ");
                sb.Append(exception.Message);
            }

            return sb.ToString();
        }

        private void AppendParameters(StringBuilder sb, MethodBase method, object[] values, bool showOutput)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Debug.Assert(parameters.Length == values.Length, "The specified method has a different number of parameters than the specified values array.");

            bool appended = false;
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((!showOutput && parameters[i].IsOut) || (showOutput && !parameters[i].ParameterType.IsByRef && !parameters[i].IsOut))
                {
                    continue;
                }

                if (appended)
                {
                    sb.Append(", ");
                }

                if (parameters[i].IsOut)
                {
                    sb.Append("out ");
                }
                else if (parameters[i].ParameterType.IsByRef)
                {
                    sb.Append("ref ");
                }

                sb.Append(parameters[i].Name);
                sb.Append(": ");
                this.parameterSerializer.Serialize(sb, values[i], parameters[i]);
                appended = true;
            }
        }
    }
}