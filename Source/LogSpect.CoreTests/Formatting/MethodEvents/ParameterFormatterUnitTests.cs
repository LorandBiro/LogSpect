namespace LogSpect.CoreTests.Formatting.MethodEvents
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using LogSpect.CoreTests.Formatting.MethodEvents.TestSubjects;
    using LogSpect.Formatting.MethodEvents;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class ParameterFormatterUnitTests
    {
        private static readonly ParameterInfo TestParameter = typeof(object).GetMethod("ToString").ReturnParameter;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullRegistry_ThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterFormatter formatter = new ParameterFormatter(null, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullFormatProvider_ThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_WithNullStringBuilder_ThrowsArgumentNullException()
        {
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), CultureInfo.InvariantCulture);
            formatter.Serialize(null, new object(), TestParameter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_WithNullParameterInfo_ThrowsArgumentNullException()
        {
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), CultureInfo.InvariantCulture);
            formatter.Serialize(new StringBuilder(), new object(), (ParameterInfo)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Serialize_WithInvalidMode_ThrowsArgumentOutOfRangeException()
        {
            TestFormatter(1, (FormattingMode)(-1), null);
        }

        [TestMethod]
        public void Serialize_NullTests()
        {
            TestFormatter(null, FormattingMode.Default, "null");
            TestFormatter(null, FormattingMode.Members, "null");
            TestFormatter(null, FormattingMode.Items, "null");
            TestFormatter(null, FormattingMode.ItemsMembers, "null");
            TestFormatter(null, FormattingMode.DoNotLog, "null");
        }

        [TestMethod]
        public void Serialize_ComplexTests()
        {
            Complex complex = new Complex { Re = 1, Im = 2 };
            TestFormatter(complex, FormattingMode.Default, "1 + 2i");
            TestFormatter(complex, FormattingMode.Members, "{ Re: 1, Im: 2 }");
            TestFormatter(complex, FormattingMode.Items, "1 + 2i");
            TestFormatter(complex, FormattingMode.ItemsMembers, "{ Re: 1, Im: 2 }");
            TestFormatter(complex, FormattingMode.DoNotLog, "-");
        }

        [TestMethod]
        public void Serialize_StringTests()
        {
            const string Str = "asdf";
            TestFormatter(Str, FormattingMode.Default, "\"asdf\"");
            TestFormatter(Str, FormattingMode.Members, "{ Length: 4 }");
            TestFormatter(Str, FormattingMode.Items, "\"asdf\"");
            TestFormatter(Str, FormattingMode.ItemsMembers, "{ Length: 4 }");
            TestFormatter(Str, FormattingMode.DoNotLog, "-");
        }

        [TestMethod]
        public void Serialize_FormattableTests()
        {
            IFormattable formattable = 3.14;
            TestFormatter(formattable, FormattingMode.Default, "3.14", CultureInfo.InvariantCulture);
            TestFormatter(formattable, FormattingMode.Default, "3,14", CultureInfo.GetCultureInfo("hu-HU"));
            TestFormatter(formattable, FormattingMode.DoNotLog, "-");
        }

        [TestMethod]
        public void Serialize_DateTimeTests()
        {
            TestFormatter(new DateTime(2009, 6, 15, 13, 45, 30), FormattingMode.Default, "2009-06-15T13:45:30.0000000");
            TestFormatter(new DateTime(2009, 6, 15, 13, 45, 30), FormattingMode.DoNotLog, "-");

            TestFormatter(new DateTime(2009, 6, 15, 13, 45, 30, DateTimeKind.Utc), FormattingMode.Default, "2009-06-15T13:45:30.0000000Z");
        }

        [TestMethod]
        public void Serialize_DateTimeOffsetTests()
        {
            TestFormatter(new DateTimeOffset(2009, 6, 15, 13, 45, 30, TimeSpan.FromHours(-7.0)), FormattingMode.Default, "2009-06-15T13:45:30.0000000-07:00");
            TestFormatter(new DateTimeOffset(2009, 6, 15, 13, 45, 30, TimeSpan.FromHours(-7.0)), FormattingMode.DoNotLog, "-");
        }

        [TestMethod]
        public void Serialize_CustomValueFormatterTests()
        {
            Complex complex = new Complex();
            TestFormatter(complex, FormattingMode.Default, "complex", null, new TestValueFormatter());
            TestFormatter(complex, FormattingMode.DoNotLog, "-");
        }

        [TestMethod]
        public void Serialize_ListTests()
        {
            List<Complex> list = new List<Complex> { new Complex { Re = 1, Im = 2 }, new Complex { Re = 2, Im = -1 } };
            TestFormatter(list, FormattingMode.Items, "[1 + 2i, 2 - 1i]");
            TestFormatter(list, FormattingMode.ItemsMembers, "[{ Re: 1, Im: 2 }, { Re: 2, Im: -1 }]");
            TestFormatter(list, FormattingMode.DoNotLog, "-");
        }

        [TestMethod]
        public void Serialize_DictionaryTests()
        {
            Dictionary<int, Complex> dict = new Dictionary<int, Complex> { { 1, new Complex { Re = 1, Im = 2 } }, { 2, new Complex { Re = 2, Im = -1 } } };
            TestFormatter(dict, FormattingMode.Items, "[1 => 1 + 2i, 2 => 2 - 1i]");
            TestFormatter(dict, FormattingMode.ItemsMembers, "[1 => { Re: 1, Im: 2 }, 2 => { Re: 2, Im: -1 }]");
            TestFormatter(dict, FormattingMode.DoNotLog, "-");
        }

        private static void TestFormatter(object value, FormattingMode mode, string expectedOutput, IFormatProvider formatProvider = null, ICustomValueFormatter customValueFormatter = null)
        {
            // Arrange
            IFormattingModeReader reader = Substitute.For<IFormattingModeReader>();
            reader.ReadMode(TestParameter).Returns(mode);

            IParameterFormatter formatter = new ParameterFormatter(reader, formatProvider ?? CultureInfo.InvariantCulture, customValueFormatter);

            StringBuilder sb = new StringBuilder();

            // Act
            formatter.Serialize(sb, value, TestParameter);

            // Assert
            Assert.AreEqual(expectedOutput, sb.ToString());
        }

        private class TestValueFormatter : ICustomValueFormatter
        {
            public bool TrySerialize(StringBuilder sb, object value, IFormatProvider formatProvider)
            {
                sb.Append("complex");
                return true;
            }
        }
    }
}
