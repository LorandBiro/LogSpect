namespace LogSpect.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class CachedFormattingModeReader : IFormattingModeReader
    {
        private readonly IFormattingModeReader baseReader;

        private readonly Dictionary<PropertyInfo, FormattingMode> propertyModes = new Dictionary<PropertyInfo, FormattingMode>();

        private readonly Dictionary<ParameterInfo, FormattingMode> parameterModes = new Dictionary<ParameterInfo, FormattingMode>();

        public CachedFormattingModeReader(IFormattingModeReader baseReader)
        {
            if (baseReader == null)
            {
                throw new ArgumentNullException("baseReader");
            }

            this.baseReader = baseReader;
        }

        public FormattingMode ReadMode(PropertyInfo property)
        {
            return GetOrAdd(this.propertyModes, property, x => this.baseReader.ReadMode(x));
        }

        public FormattingMode ReadMode(ParameterInfo parameter)
        {
            return GetOrAdd(this.parameterModes, parameter, x => this.baseReader.ReadMode(x));
        }

        private static FormattingMode GetOrAdd<TKey>(Dictionary<TKey, FormattingMode> modes, TKey key, Func<TKey, FormattingMode> valueFactory)
        {
            lock (modes)
            {
                FormattingMode value;
                if (!modes.TryGetValue(key, out value))
                {
                    value = valueFactory(key);
                    modes.Add(key, value);
                }

                return value;
            }
        }
    }
}