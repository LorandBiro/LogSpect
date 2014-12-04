namespace LogSpect.CoreTests.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using LogSpect.CoreTests.Formatting.TestSubjects;
    using LogSpect.Formatting;
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
            IParameterFormatter formatter = new ParameterFormatter(null, Substitute.For<ICustomFormatterService>(), CultureInfo.InvariantCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullCustomFormatterService_ThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), null, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullFormatProvider_ThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), Substitute.For<ICustomFormatterService>(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_WithNullStringBuilder_ThrowsArgumentNullException()
        {
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), Substitute.For<ICustomFormatterService>(), CultureInfo.InvariantCulture);
            formatter.Serialize(null, new object(), TestParameter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_WithNullParameterInfo_ThrowsArgumentNullException()
        {
            IParameterFormatter formatter = new ParameterFormatter(Substitute.For<IFormattingModeReader>(), Substitute.For<ICustomFormatterService>(), CultureInfo.InvariantCulture);
            formatter.Serialize(new StringBuilder(), new object(), null);
        }

        [TestMethod]
        public void Serialize_NullTests()
        {
            TestFormatter(null, FormattingMode.Default, "null");
            TestFormatter(null, FormattingMode.Members, "null");
            TestFormatter(null, FormattingMode.Items, "null");
            TestFormatter(null, FormattingMode.ItemsMembers, "null");
        }

        [TestMethod]
        public void Serialize_ComplexTests()
        {
            Complex complex = new Complex { Re = 1, Im = 2 };
            TestFormatter(complex, FormattingMode.Default, "1 + 2i");
            TestFormatter(complex, FormattingMode.Members, "{ Re: 1, Im: 2 }");
            TestFormatter(complex, FormattingMode.Items, "1 + 2i");
            TestFormatter(complex, FormattingMode.ItemsMembers, "{ Re: 1, Im: 2 }");
        }

        [TestMethod]
        public void Serialize_StringTests()
        {
            const string Str = "asdf";
            TestFormatter(Str, FormattingMode.Default, "\"asdf\"");
            TestFormatter(Str, FormattingMode.Members, "{ Length: 4 }");
            TestFormatter(Str, FormattingMode.Items, "\"asdf\"");
            TestFormatter(Str, FormattingMode.ItemsMembers, "{ Length: 4 }");
        }

        [TestMethod]
        public void Serialize_FormattableTests()
        {
            IFormattable formattable = new DateTime(2000, 1, 1, 0, 0, 0);
            TestFormatter(formattable, FormattingMode.Default, "01/01/2000 00:00:00", null, CultureInfo.InvariantCulture);
            TestFormatter(formattable, FormattingMode.Default, "1/1/2000 12:00:00 AM", null, CultureInfo.GetCultureInfo("en-US"));
        }

        [TestMethod]
        public void Serialize_CustomFormatterTests()
        {
            Complex complex = new Complex();

            ICustomFormatterService customFormatterService = Substitute.For<ICustomFormatterService>();
            customFormatterService.TrySerialize(Arg.Any<StringBuilder>(), complex, Arg.Any<CultureInfo>()).Returns(
                x =>
                    {
                        ((StringBuilder)x[0]).Append("complex");
                        return true;
                    });

            TestFormatter(complex, FormattingMode.Default, "complex", customFormatterService);
        }

        [TestMethod]
        public void Serialize_ListTests()
        {
            List<Complex> list = new List<Complex> { new Complex { Re = 1, Im = 2 }, new Complex { Re = 2, Im = -1 } };
            TestFormatter(list, FormattingMode.Items, "[1 + 2i, 2 - 1i]");
            TestFormatter(list, FormattingMode.ItemsMembers, "[{ Re: 1, Im: 2 }, { Re: 2, Im: -1 }]");
        }

        [TestMethod]
        public void Serialize_DictionaryTests()
        {
            Dictionary<int, Complex> dict = new Dictionary<int, Complex> { { 1, new Complex { Re = 1, Im = 2 } }, { 2, new Complex { Re = 2, Im = -1 } } };
            TestFormatter(dict, FormattingMode.Items, "[1 => 1 + 2i, 2 => 2 - 1i]");
            TestFormatter(dict, FormattingMode.ItemsMembers, "[1 => { Re: 1, Im: 2 }, 2 => { Re: 2, Im: -1 }]");
        }

        private static void TestFormatter(object value, FormattingMode mode, string expectedOutput, ICustomFormatterService customFormatterService = null, IFormatProvider formatProvider = null)
        {
            // Arrange
            IFormattingModeReader reader = Substitute.For<IFormattingModeReader>();
            reader.ReadMode(TestParameter).Returns(mode);

            IParameterFormatter formatter = new ParameterFormatter(
                reader,
                customFormatterService ?? Substitute.For<ICustomFormatterService>(),
                formatProvider ?? CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            // Act
            formatter.Serialize(sb, value, TestParameter);

            // Assert
            Assert.AreEqual(expectedOutput, sb.ToString());
        }
    }
}
