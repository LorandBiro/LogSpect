namespace LogSpect.CoreTests.Formatting
{
    using System;
    using System.Reflection;
    using LogSpect.Formatting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class CachedFormattingModeReaderUnitTests
    {
        private static readonly PropertyInfo TestProperty = typeof(string).GetProperty("Length");

        private static readonly ParameterInfo TestParameter = typeof(object).GetMethod("ToString").ReturnParameter;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullBaseReaderThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IFormattingModeReader reader = new CachedFormattingModeReader(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadModeWithNullParameterInfoThrowsArgumentNullException()
        {
            IFormattingModeReader reader = new CachedFormattingModeReader(Substitute.For<IFormattingModeReader>());
            reader.ReadMode((ParameterInfo)null);
        }

        [TestMethod]
        public void ReadModeWithParameterInfoReturnsWrappedObjectsResult()
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
        public void ReadModeWithParameterInfoCallsReaderOnlyOnce()
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
        public void ReadModeWithNullPropertyInfoThrowsArgumentNullException()
        {
            IFormattingModeReader reader = new CachedFormattingModeReader(Substitute.For<IFormattingModeReader>());
            reader.ReadMode((PropertyInfo)null);
        }

        [TestMethod]
        public void ReadModeWithPropertyInfoInfoReturnsWrappedObjectsResult()
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
        public void ReadModeWithPropertyInfoCallsReaderOnlyOnce()
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
