namespace LogSpectTests.Serialization
{
    using System;
    using System.Reflection;
    using LogSpect.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class CachingSerializationModeReaderUnitTests
    {
        private static readonly PropertyInfo TestProperty = typeof(string).GetProperty("Length");

        private static readonly ParameterInfo TestParameter = typeof(object).GetMethod("ToString").ReturnParameter;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullReaderThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            ISerializationModeReader reader = new CachingSerializationModeReader(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadSerializationModeWithNullParameterInfoThrowsArgumentNullException()
        {
            ISerializationModeReader reader = new SerializationModeReader();
            reader.ReadSerializationMode((ParameterInfo)null);
        }

        [TestMethod]
        public void ReadSerializationModeWithParameterInfoReturnsWrappedObjectsResult()
        {
            // Arrange
            ISerializationModeReader reader = Substitute.For<ISerializationModeReader>();
            reader.ReadSerializationMode(TestParameter).Returns(SerializationMode.ItemsMembers);

            ISerializationModeReader wrapper = new CachingSerializationModeReader(reader);

            // Act
            SerializationMode mode = wrapper.ReadSerializationMode(TestParameter);

            // Assert
            Assert.AreEqual(SerializationMode.ItemsMembers, mode);
        }

        [TestMethod]
        public void ReadSerializationModeWithParameterInfoCallsReaderOnlyOnce()
        {
            // Arrange
            ISerializationModeReader reader = Substitute.For<ISerializationModeReader>();
            ISerializationModeReader wrapper = new CachingSerializationModeReader(reader);

            // Act
            wrapper.ReadSerializationMode(TestParameter);
            wrapper.ReadSerializationMode(TestParameter);
            wrapper.ReadSerializationMode(TestParameter);

            // Assert
            reader.Received(1).ReadSerializationMode(TestParameter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadSerializationModeWithNullPropertyInfoThrowsArgumentNullException()
        {
            ISerializationModeReader reader = new SerializationModeReader();
            reader.ReadSerializationMode((PropertyInfo)null);
        }

        [TestMethod]
        public void ReadSerializationModeWithPropertyInfoInfoReturnsWrappedObjectsResult()
        {
            // Arrange
            ISerializationModeReader reader = Substitute.For<ISerializationModeReader>();
            reader.ReadSerializationMode(TestProperty).Returns(SerializationMode.ItemsMembers);

            ISerializationModeReader wrapper = new CachingSerializationModeReader(reader);

            // Act
            SerializationMode mode = wrapper.ReadSerializationMode(TestProperty);

            // Assert
            Assert.AreEqual(SerializationMode.ItemsMembers, mode);
        }

        [TestMethod]
        public void ReadSerializationModeWithPropertyInfoCallsReaderOnlyOnce()
        {
            // Arrange
            ISerializationModeReader reader = Substitute.For<ISerializationModeReader>();
            ISerializationModeReader wrapper = new CachingSerializationModeReader(reader);

            // Act
            wrapper.ReadSerializationMode(TestProperty);
            wrapper.ReadSerializationMode(TestProperty);
            wrapper.ReadSerializationMode(TestProperty);

            // Assert
            reader.Received(1).ReadSerializationMode(TestProperty);
        }
    }
}
