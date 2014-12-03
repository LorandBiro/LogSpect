namespace LogSpect.CoreTests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using LogSpect.CoreTests.Serialization.TestSubjects;
    using LogSpect.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class ParameterSerializerUnitTests
    {
        private static readonly ParameterInfo TestParameter = typeof(object).GetMethod("ToString").ReturnParameter;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullRegistryThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterSerializer serializer = new ParameterSerializer(null, Substitute.For<ICustomSerializerService>(), CultureInfo.InvariantCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullCustomSerializerServiceThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterSerializer serializer = new ParameterSerializer(Substitute.For<ISerializationModeReader>(), null, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullFormatProviderThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IParameterSerializer serializer = new ParameterSerializer(Substitute.For<ISerializationModeReader>(), Substitute.For<ICustomSerializerService>(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeWithNullStringBuilderThrowsArgumentNullException()
        {
            IParameterSerializer serializer = new ParameterSerializer(Substitute.For<ISerializationModeReader>(), Substitute.For<ICustomSerializerService>(), CultureInfo.InvariantCulture);
            serializer.Serialize(null, new object(), TestParameter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeWithNullParameterInfoThrowsArgumentNullException()
        {
            IParameterSerializer serializer = new ParameterSerializer(Substitute.For<ISerializationModeReader>(), Substitute.For<ICustomSerializerService>(), CultureInfo.InvariantCulture);
            serializer.Serialize(new StringBuilder(), new object(), null);
        }

        [TestMethod]
        public void NullTests()
        {
            TestSerialization(null, SerializationMode.Default, "null");
            TestSerialization(null, SerializationMode.Members, "null");
            TestSerialization(null, SerializationMode.Items, "null");
            TestSerialization(null, SerializationMode.ItemsMembers, "null");
        }

        [TestMethod]
        public void ComplexTests()
        {
            Complex complex = new Complex { Re = 1, Im = 2 };
            TestSerialization(complex, SerializationMode.Default, "1 + 2i");
            TestSerialization(complex, SerializationMode.Members, "{ Re: 1, Im: 2 }");
            TestSerialization(complex, SerializationMode.Items, "1 + 2i");
            TestSerialization(complex, SerializationMode.ItemsMembers, "{ Re: 1, Im: 2 }");
        }

        [TestMethod]
        public void StringTests()
        {
            const string Str = "asdf";
            TestSerialization(Str, SerializationMode.Default, "\"asdf\"");
            TestSerialization(Str, SerializationMode.Members, "{ Length: 4 }");
            TestSerialization(Str, SerializationMode.Items, "\"asdf\"");
            TestSerialization(Str, SerializationMode.ItemsMembers, "{ Length: 4 }");
        }

        [TestMethod]
        public void FormattableTests()
        {
            IFormattable formattable = new DateTime(2000, 1, 1, 0, 0, 0);
            TestSerialization(formattable, SerializationMode.Default, "01/01/2000 00:00:00", null, CultureInfo.InvariantCulture);
            TestSerialization(formattable, SerializationMode.Default, "1/1/2000 12:00:00 AM", null, CultureInfo.GetCultureInfo("en-US"));
        }

        [TestMethod]
        public void CustomSerializerTests()
        {
            Complex complex = new Complex();

            ICustomSerializerService customSerializerService = Substitute.For<ICustomSerializerService>();
            customSerializerService.TrySerialize(Arg.Any<StringBuilder>(), complex, Arg.Any<CultureInfo>()).Returns(
                x =>
                    {
                        ((StringBuilder)x[0]).Append("complex");
                        return true;
                    });

            TestSerialization(complex, SerializationMode.Default, "complex", customSerializerService);
        }

        [TestMethod]
        public void ListTests()
        {
            List<Complex> list = new List<Complex> { new Complex { Re = 1, Im = 2 }, new Complex { Re = 2, Im = -1 } };
            TestSerialization(list, SerializationMode.Items, "[1 + 2i, 2 - 1i]");
            TestSerialization(list, SerializationMode.ItemsMembers, "[{ Re: 1, Im: 2 }, { Re: 2, Im: -1 }]");
        }

        [TestMethod]
        public void DictionaryTests()
        {
            Dictionary<int, Complex> dict = new Dictionary<int, Complex> { { 1, new Complex { Re = 1, Im = 2 } }, { 2, new Complex { Re = 2, Im = -1 } } };
            TestSerialization(dict, SerializationMode.Items, "[1 => 1 + 2i, 2 => 2 - 1i]");
            TestSerialization(dict, SerializationMode.ItemsMembers, "[1 => { Re: 1, Im: 2 }, 2 => { Re: 2, Im: -1 }]");
        }

        private static void TestSerialization(object value, SerializationMode mode, string expectedOutput, ICustomSerializerService customSerializerService = null, IFormatProvider formatProvider = null)
        {
            // Arrange
            ISerializationModeReader reader = Substitute.For<ISerializationModeReader>();
            reader.ReadSerializationMode(TestParameter).Returns(mode);

            IParameterSerializer serializer = new ParameterSerializer(
                reader,
                customSerializerService ?? Substitute.For<ICustomSerializerService>(),
                formatProvider ?? CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            // Act
            serializer.Serialize(sb, value, TestParameter);

            // Assert
            Assert.AreEqual(expectedOutput, sb.ToString());
        }
    }
}
