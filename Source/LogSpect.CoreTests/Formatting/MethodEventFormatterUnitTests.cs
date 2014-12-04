namespace LogSpect.CoreTests.Formatting
{
    using System;
    using System.Reflection;
    using System.Text;
    using LogSpect.Formatting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class MethodEventFormatterUnitTests
    {
        private static readonly MethodBase Method1 = typeof(TestSubject).GetMethod("TestMethod1");

        private static readonly MethodBase Method2 = typeof(TestSubject).GetMethod("TestMethod2");

        private static readonly MethodBase Method3 = typeof(TestSubject).GetMethod("TestMethod3");

        private static readonly MethodBase Method4 = typeof(TestSubject).GetMethod("TestMethod4");

        private static readonly MethodBase Method5 = typeof(TestSubject).GetMethod("TestMethod5");

        private static readonly MethodBase Method6 = typeof(TestSubject).GetMethod("TestMethod6");

        private static readonly MethodBase Method7 = typeof(TestSubject).GetMethod("TestMethod7");

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNull_ThrowsArgumentNullException()
        {
            // ReSharper disable once UnusedVariable
            IMethodEventFormatter formatter = new MethodEventFormatter(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeEnter_WithNullMethod_ThrowsArgumentNullException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeEnter(null, new object[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeEnter_WithNullParameters_ThrowsArgumentNullException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeEnter(Method1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SerializeEnter_WithParameterCountMismatch_ThrowsArgumentException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeEnter(Method1, new object[100]);
        }

        [TestMethod]
        public void SerializeEnter_Tests()
        {
            IMethodEventFormatter formatter = CreateTestMethodEventFormatter();

            Assert.AreEqual("Enter TestSubject.TestMethod1()", formatter.SerializeEnter(Method1, new object[0]));
            Assert.AreEqual("Enter TestSubject.TestMethod2(p1: X)", formatter.SerializeEnter(Method2, new object[1]));
            Assert.AreEqual("Enter TestSubject.TestMethod3(p1: X, p2: X)", formatter.SerializeEnter(Method3, new object[2]));
            Assert.AreEqual("Enter TestSubject.TestMethod4(p1: X, p2: X, p3: X)", formatter.SerializeEnter(Method4, new object[3]));
            Assert.AreEqual("Enter TestSubject.TestMethod5(p1: X, p2: X, p3: X)", formatter.SerializeEnter(Method5, new object[3]));
            Assert.AreEqual("Enter TestSubject.TestMethod6(p1: X, p2: X, ref p3: X)", formatter.SerializeEnter(Method6, new object[3]));
            Assert.AreEqual("Enter TestSubject.TestMethod7(p1: X, p2: X)", formatter.SerializeEnter(Method7, new object[3]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeLeave_WithNullMethod_ThrowsArgumentNullException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeLeave(null, new object[0], null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeLeave_WithNullParameters_ThrowsArgumentNullException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeLeave(Method1, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SerializeLeave_WithParameterCountMismatch_ThrowsArgumentException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeLeave(Method1, new object[100], null);
        }

        [TestMethod]
        public void SerializeLeave_Tests()
        {
            IMethodEventFormatter formatter = CreateTestMethodEventFormatter();

            Assert.AreEqual("Leave TestSubject.TestMethod1()", formatter.SerializeLeave(Method1, new object[0], null));
            Assert.AreEqual("Leave TestSubject.TestMethod2()", formatter.SerializeLeave(Method2, new object[1], null));
            Assert.AreEqual("Leave TestSubject.TestMethod3()", formatter.SerializeLeave(Method3, new object[2], null));
            Assert.AreEqual("Leave TestSubject.TestMethod4()", formatter.SerializeLeave(Method4, new object[3], null));
            Assert.AreEqual("Leave TestSubject.TestMethod5(): X", formatter.SerializeLeave(Method5, new object[3], null));
            Assert.AreEqual("Leave TestSubject.TestMethod6(ref p3: X): X", formatter.SerializeLeave(Method6, new object[3], null));
            Assert.AreEqual("Leave TestSubject.TestMethod7(out p3: X): X", formatter.SerializeLeave(Method7, new object[3], null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeException_WithNullMethod_ThrowsArgumentNullException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeException(null, new Exception(), false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeException_WithNullException_ThrowsArgumentNullException()
        {
            IMethodEventFormatter formatter = new MethodEventFormatter(Substitute.For<IParameterFormatter>());
            formatter.SerializeException(Method1, null, false);
        }

        [TestMethod]
        public void SerializeException_Tests()
        {
            IMethodEventFormatter formatter = CreateTestMethodEventFormatter();

            Assert.AreEqual("TestSubject.TestMethod1() failed with ArgumentException! Message: Lorem ipsum", formatter.SerializeException(Method1, new ArgumentException("Lorem ipsum"), false));
            Assert.AreEqual("TestSubject.TestMethod1() threw ArgumentException. Message: Lorem ipsum", formatter.SerializeException(Method1, new ArgumentException("Lorem ipsum"), true));
        }

        private static IMethodEventFormatter CreateTestMethodEventFormatter()
        {
            IParameterFormatter parameterFormatter = Substitute.For<IParameterFormatter>();
            parameterFormatter.When(x => x.Serialize(Arg.Any<StringBuilder>(), Arg.Any<object>(), Arg.Any<ParameterInfo>())).Do(x => ((StringBuilder)x[0]).Append("X"));

            IMethodEventFormatter methodEventFormatter = new MethodEventFormatter(parameterFormatter);
            return methodEventFormatter;
        }

        private class TestSubject
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedParameter.Local
            public void TestMethod1()
            {
            }

            public void TestMethod2(object p1)
            {
            }

            public void TestMethod3(object p1, object p2)
            {
            }

            public void TestMethod4(object p1, object p2, object p3)
            {
            }

            public object TestMethod5(object p1, object p2, object p3)
            {
                return null;
            }

            public object TestMethod6(object p1, object p2, ref object p3)
            {
                return null;
            }

            public object TestMethod7(object p1, object p2, out object p3)
            {
                p3 = null;
                return null;
            }

            // ReSharper restore UnusedMember.Local
            // ReSharper restore UnusedParameter.Local
        }
    }
}
