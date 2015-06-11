namespace LogSpect.CoreTests.Formatting.MethodEvents
{
    using System;
    using System.Reflection;
    using LogSpect.Formatting.MethodEvents;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class CachedFormattingModeReaderUnitTests
    {
        private static readonly PropertyInfo TestProperty = typeof(string).GetProperty("Length");

        private static readonly ParameterInfo TestParameter = typeof(object).GetMethod("ToString").ReturnParameter;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNull_ThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IFormattingModeReader reader = new CachedFormattingModeReader(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadModeParameterInfo_WithNull_ThrowsArgumentNullException()
        {
            IFormattingModeReader reader = new CachedFormattingModeReader(Substitute.For<IFormattingModeReader>());
            reader.ReadMode((ParameterInfo)null);
        }

        [TestMethod]
        public void ReadModeParameterInfo_ReturnsTheResultOfTheBaseReader()
        {
            // Arrange
            IFormattingModeReader baseReader = Substitute.For<IFormattingModeReader>();
            baseReader.ReadMode(TestParameter).Returns(FormattingMode.ItemsMembers);

            IFormattingModeReader reader = new CachedFormattingModeReader(baseReader);

            // Act
            FormattingMode mode = reader.ReadMode(TestParameter);

            // Assert
            Assert.AreEqual(FormattingMode.ItemsMembers, mode);
        }

        [TestMethod]
        public void ReadModeParameterInfo_CallsTheBaseReaderAtMostOnce()
        {
            // Arrange
            IFormattingModeReader baseReader = Substitute.For<IFormattingModeReader>();
            IFormattingModeReader reader = new CachedFormattingModeReader(baseReader);

            // Act
            reader.ReadMode(TestParameter);
            reader.ReadMode(TestParameter);
            reader.ReadMode(TestParameter);

            // Assert
            baseReader.Received(1).ReadMode(TestParameter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadModePropertyInfo_WithNull_ThrowsArgumentNullException()
        {
            IFormattingModeReader reader = new CachedFormattingModeReader(Substitute.For<IFormattingModeReader>());
            reader.ReadMode((PropertyInfo)null);
        }

        [TestMethod]
        public void ReadModePropertyInfo_ReturnsTheResultOfTheBaseReader()
        {
            // Arrange
            IFormattingModeReader baseReader = Substitute.For<IFormattingModeReader>();
            baseReader.ReadMode(TestProperty).Returns(FormattingMode.ItemsMembers);

            IFormattingModeReader reader = new CachedFormattingModeReader(baseReader);

            // Act
            FormattingMode mode = reader.ReadMode(TestProperty);

            // Assert
            Assert.AreEqual(FormattingMode.ItemsMembers, mode);
        }

        [TestMethod]
        public void ReadModePropertyInfo_CallsTheBaseReaderAtMostOnce()
        {
            // Arrange
            IFormattingModeReader baseReader = Substitute.For<IFormattingModeReader>();
            IFormattingModeReader reader = new CachedFormattingModeReader(baseReader);

            // Act
            reader.ReadMode(TestProperty);
            reader.ReadMode(TestProperty);
            reader.ReadMode(TestProperty);

            // Assert
            baseReader.Received(1).ReadMode(TestProperty);
        }
    }
}
