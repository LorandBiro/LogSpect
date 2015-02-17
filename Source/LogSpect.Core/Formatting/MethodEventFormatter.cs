namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;
    using System.Text;

    public sealed class MethodEventFormatter : IMethodEventFormatter
    {
        private readonly IParameterFormatter parameterFormatter;

        public MethodEventFormatter(IParameterFormatter parameterFormatter)
        {
            if (parameterFormatter == null)
            {
                throw new ArgumentNullException("parameterFormatter");
            }

            this.parameterFormatter = parameterFormatter;
        }

        public string SerializeEnter(Type type, MethodBase method, object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Enter ");
            sb.Append(type.Name);
            sb.Append(".");
            sb.Append(method.Name);
            sb.Append("(");
            this.AppendParameters(sb, method, parameters, false);
            sb.Append(")");

            return sb.ToString();
        }

        public string SerializeLeave(Type type, MethodBase method, object[] parameters, object returnValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Leave ");
            sb.Append(type.Name);
            sb.Append(".");
            sb.Append(method.Name);
            sb.Append("(");
            this.AppendParameters(sb, method, parameters, true);
            sb.Append(")");

            MethodInfo methodInfo = method as MethodInfo;
            if (methodInfo != null)
            {
                // Constructors are represented by the RuntimeConstructorInfo type and do not have return values.
                // (IsConstructor returns false for static constructors.)
                if (methodInfo.ReturnType != typeof(void))
                {
                    sb.Append(": ");
                    this.parameterFormatter.Serialize(sb, returnValue, methodInfo.ReturnParameter);
                }
            }

            return sb.ToString();
        }

        public string SerializeException(Type type, MethodBase method, Exception exception, bool expected)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            StringBuilder sb = new StringBuilder();
            if (expected)
            {
                sb.Append(type.Name);
                sb.Append(".");
                sb.Append(method.Name);
                sb.Append("() threw ");
                sb.Append(exception.GetType().Name);
                sb.Append(". Message: ");
                sb.Append(exception.Message);
            }
            else
            {
                sb.Append(type.Name);
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
            if (parameters.Length != values.Length)
            {
                throw new ArgumentException("The specified method has a different number of parameters than the specified values array.");
            }

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
                this.parameterFormatter.Serialize(sb, values[i], parameters[i]);
                appended = true;
            }
        }
    }
}