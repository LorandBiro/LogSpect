namespace LogSpect.CoreTests.Formatting
{
    using System;
    using System.Reflection;
    using LogSpect;
    using LogSpect.Formatting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FormattingModeReaderUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadModePropertyInfo_WithNull_ThrowsArgumentNullException()
        {
            IFormattingModeReader reader = new FormattingModeReader();
            reader.ReadMode((PropertyInfo)null);
        }

        [TestMethod]
        public void ReadModePropertyInfo_Tests()
        {
            IFormattingModeReader reader = new FormattingModeReader();
            PropertyInfo property1 = typeof(TestSubject).GetProperty("PropertyTest1");
            PropertyInfo property2 = typeof(TestSubject).GetProperty("PropertyTest2");
            PropertyInfo property3 = typeof(TestSubject).GetProperty("PropertyTest3");
            PropertyInfo property4 = typeof(TestSubject).GetProperty("PropertyTest4");
            PropertyInfo property5 = typeof(TestSubject).GetProperty("PropertyTest5");
            PropertyInfo property6 = typeof(TestSubject).GetProperty("PropertyTest6");
            PropertyInfo property7 = typeof(TestSubject).GetProperty("PropertyTest7");
            PropertyInfo property8 = typeof(TestSubject).GetProperty("PropertyTest8");

            Assert.AreEqual(FormattingMode.Default, reader.ReadMode(property1));
            Assert.AreEqual(FormattingMode.Members, reader.ReadMode(property2));
            Assert.AreEqual(FormattingMode.Items, reader.ReadMode(property3));
            Assert.AreEqual(FormattingMode.ItemsMembers, reader.ReadMode(property4));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(property5));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(property6));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(property7));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(property8));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadModeParameterInfo_WithNull_ThrowsArgumentNullException()
        {
            IFormattingModeReader reader = new FormattingModeReader();
            reader.ReadMode((ParameterInfo)null);
        }

        [TestMethod]
        public void ReadModeParameterInfo_Tests()
        {
            IFormattingModeReader reader = new FormattingModeReader();
            ParameterInfo[] parameters = typeof(TestSubject).GetMethod("ParameterTests").GetParameters();
            ParameterInfo returnParameter1 = typeof(TestSubject).GetMethod("ReturnValueTest1").ReturnParameter;
            ParameterInfo returnParameter2 = typeof(TestSubject).GetMethod("ReturnValueTest2").ReturnParameter;
            ParameterInfo returnParameter3 = typeof(TestSubject).GetMethod("ReturnValueTest3").ReturnParameter;
            ParameterInfo returnParameter4 = typeof(TestSubject).GetMethod("ReturnValueTest4").ReturnParameter;
            ParameterInfo returnParameter5 = typeof(TestSubject).GetMethod("ReturnValueTest5").ReturnParameter;
            ParameterInfo returnParameter6 = typeof(TestSubject).GetMethod("ReturnValueTest6").ReturnParameter;
            ParameterInfo returnParameter7 = typeof(TestSubject).GetMethod("ReturnValueTest7").ReturnParameter;
            ParameterInfo returnParameter8 = typeof(TestSubject).GetMethod("ReturnValueTest8").ReturnParameter;

            Assert.AreEqual(FormattingMode.Default, reader.ReadMode(parameters[0]));
            Assert.AreEqual(FormattingMode.Members, reader.ReadMode(parameters[1]));
            Assert.AreEqual(FormattingMode.Items, reader.ReadMode(parameters[2]));
            Assert.AreEqual(FormattingMode.ItemsMembers, reader.ReadMode(parameters[3]));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(parameters[4]));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(parameters[5]));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(parameters[6]));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(parameters[7]));

            Assert.AreEqual(FormattingMode.Default, reader.ReadMode(returnParameter1));
            Assert.AreEqual(FormattingMode.Members, reader.ReadMode(returnParameter2));
            Assert.AreEqual(FormattingMode.Items, reader.ReadMode(returnParameter3));
            Assert.AreEqual(FormattingMode.ItemsMembers, reader.ReadMode(returnParameter4));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(returnParameter5));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(returnParameter6));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(returnParameter7));
            Assert.AreEqual(FormattingMode.DoNotLog, reader.ReadMode(returnParameter8));
        }

        private class TestSubject
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedParameter.Local
            public object PropertyTest1 { get; set; }

            [LogMembers]
            public object PropertyTest2 { get; set; }

            [LogItems]
            public object PropertyTest3 { get; set; }

            [LogItems]
            [LogMembers]
            public object PropertyTest4 { get; set; }

            [DoNotLog]
            public object PropertyTest5 { get; set; }

            [LogMembers]
            [DoNotLog]
            public object PropertyTest6 { get; set; }

            [LogItems]
            [DoNotLog]
            public object PropertyTest7 { get; set; }

            [LogItems]
            [LogMembers]
            [DoNotLog]
            public object PropertyTest8 { get; set; }

            public object ReturnValueTest1()
            {
                return null;
            }

            [return: LogMembers]
            public object ReturnValueTest2()
            {
                return null;
            }

            [return: LogItems]
            public object ReturnValueTest3()
            {
                return null;
            }

            [return: LogItems]
            [return: LogMembers]
            public object ReturnValueTest4()
            {
                return null;
            }

            [return: DoNotLog]
            public object ReturnValueTest5()
            {
                return null;
            }

            [return: LogMembers]
            [return: DoNotLog]
            public object ReturnValueTest6()
            {
                return null;
            }

            [return: LogItems]
            [return: DoNotLog]
            public object ReturnValueTest7()
            {
                return null;
            }

            [return: LogItems]
            [return: LogMembers]
            [return: DoNotLog]
            public object ReturnValueTest8()
            {
                return null;
            }

            public void ParameterTests(
                object a,
                [LogMembers] object b,
                [LogItems] object c,
                [LogItems] [LogMembers] object d,
                [DoNotLog] object e,
                [LogMembers] [DoNotLog] object f,
                [LogItems] [DoNotLog] object g,
                [LogItems] [LogMembers] [DoNotLog] object h)
            {
            }

            // ReSharper restore UnusedMember.Local
            // ReSharper restore UnusedParameter.Local
        }
    }
}
