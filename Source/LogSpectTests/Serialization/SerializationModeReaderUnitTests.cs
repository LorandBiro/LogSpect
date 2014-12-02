namespace LogSpectTests.Serialization
{
    using System;
    using System.Reflection;
    using LogSpect;
    using LogSpect.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializationModeReaderUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadSerializationModeWithNullPropertyInfoThrowsArgumentNullException()
        {
            ISerializationModeReader reader = new SerializationModeReader();
            reader.ReadSerializationMode((PropertyInfo)null);
        }

        [TestMethod]
        public void ReadSerializationModeWithPropertyInfo()
        {
            ISerializationModeReader reader = new SerializationModeReader();
            PropertyInfo property1 = typeof(TestSubject).GetProperty("PropertyTest1");
            PropertyInfo property2 = typeof(TestSubject).GetProperty("PropertyTest2");
            PropertyInfo property3 = typeof(TestSubject).GetProperty("PropertyTest3");
            PropertyInfo property4 = typeof(TestSubject).GetProperty("PropertyTest4");

            Assert.AreEqual(SerializationMode.Default, reader.ReadSerializationMode(property1));
            Assert.AreEqual(SerializationMode.Members, reader.ReadSerializationMode(property2));
            Assert.AreEqual(SerializationMode.Items, reader.ReadSerializationMode(property3));
            Assert.AreEqual(SerializationMode.ItemsMembers, reader.ReadSerializationMode(property4));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadSerializationModeWithNullParameterInfoThrowsArgumentNullException()
        {
            ISerializationModeReader reader = new SerializationModeReader();
            reader.ReadSerializationMode((ParameterInfo)null);
        }

        [TestMethod]
        public void ReadSerializationModeWithParameterInfo()
        {
            ISerializationModeReader reader = new SerializationModeReader();
            ParameterInfo[] parameters = typeof(TestSubject).GetMethod("ParameterTests").GetParameters();
            ParameterInfo returnParameter1 = typeof(TestSubject).GetMethod("ReturnValueTest1").ReturnParameter;
            ParameterInfo returnParameter2 = typeof(TestSubject).GetMethod("ReturnValueTest2").ReturnParameter;
            ParameterInfo returnParameter3 = typeof(TestSubject).GetMethod("ReturnValueTest3").ReturnParameter;
            ParameterInfo returnParameter4 = typeof(TestSubject).GetMethod("ReturnValueTest4").ReturnParameter;

            Assert.AreEqual(SerializationMode.Default, reader.ReadSerializationMode(parameters[0]));
            Assert.AreEqual(SerializationMode.Members, reader.ReadSerializationMode(parameters[1]));
            Assert.AreEqual(SerializationMode.Items, reader.ReadSerializationMode(parameters[2]));
            Assert.AreEqual(SerializationMode.ItemsMembers, reader.ReadSerializationMode(parameters[3]));

            Assert.AreEqual(SerializationMode.Default, reader.ReadSerializationMode(returnParameter1));
            Assert.AreEqual(SerializationMode.Members, reader.ReadSerializationMode(returnParameter2));
            Assert.AreEqual(SerializationMode.Items, reader.ReadSerializationMode(returnParameter3));
            Assert.AreEqual(SerializationMode.ItemsMembers, reader.ReadSerializationMode(returnParameter4));
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

            public void ParameterTests(object a, [LogMembers] object b, [LogItems] object c, [LogItems] [LogMembers] object d)
            {
            }

            // ReSharper restore UnusedMember.Local
            // ReSharper restore UnusedParameter.Local
        }
    }
}
