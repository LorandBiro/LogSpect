namespace LogSpect.Formatting
{
    using System;
    using System.Reflection;

    public interface IFormattingModeReader
    {
        /// <summary>
        /// Reads the attributes of the specified property and determines how it should be serialized when the containing object is logged.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>The formatting mode.</returns>
        /// <exception cref="ArgumentNullException">If property is null.</exception>
        FormattingMode ReadMode(MemberInfo member);

        FormattingMode ReadMode(ParameterInfo parameter);
    }
}
