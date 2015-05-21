namespace LogSpectRewriterTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RewriterIntegrationTests
    {
        #region RewriteTargets

        [TestMethod]
        public void Constructor()
        {
            const string ClassDefinitions = @"
public class Foo
{
    public static bool HasRun;

    [LogCalls]
    public Foo()
    {
        HasRun = true;
    }
}";
            const string TestCode = "new Foo(); Assert.IsTrue(Foo.HasRun);";
            const string ExpectedOutput = @"  TRACE|Enter Foo..ctor()
  TRACE|Leave Foo..ctor()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void StaticConstructor()
        {
            const string ClassDefinitions = @"
internal class Foo
{
    public static bool HasRun;

    [LogCalls]
    static Foo()
    {
        HasRun = true;
    }
}";
            const string TestCode = "Assert.IsTrue(Foo.HasRun);";
            const string ExpectedOutput = @"  TRACE|Enter Foo..cctor()
  TRACE|Leave Foo..cctor()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void PropertyGetter()
        {
            const string ClassDefinitions = @"
internal class Foo
{
    public int Bar { [LogCalls] get; set; }
}";
            const string TestCode = "Foo foo = new Foo(); foo.Bar = 3; Assert.AreEqual(3, foo.Bar);";
            const string ExpectedOutput = @"  TRACE|Enter Foo.get_Bar()
  TRACE|Leave Foo.get_Bar(): 3
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void PropertySetter()
        {
            const string ClassDefinitions = @"
internal class Foo
{
    public int Bar { get; [LogCalls] set; }
}";
            const string TestCode = "Foo foo = new Foo(); foo.Bar = 3; Assert.AreEqual(3, foo.Bar);";
            const string ExpectedOutput = @"  TRACE|Enter Foo.set_Bar(value: 3)
  TRACE|Leave Foo.set_Bar()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        #endregion

        #region Inheritance

        [TestMethod]
        public void MethodsInBaseClassShowsTheNameOfTheExecutingClass()
        {
            const string ClassDefinitions = @"
internal abstract class BaseClass
{
    [LogCalls]
    public void Foo()
    {
    }
}

internal class DerivedClass : BaseClass
{
}";
            const string TestCode = "new DerivedClass().Foo();";
            const string ExpectedOutput = @"  TRACE|Enter DerivedClass.Foo()
  TRACE|Leave DerivedClass.Foo()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void StaticMethodsInBaseClassShowsTheNameOfTheBaseClass()
        {
            const string ClassDefinitions = @"
internal abstract class BaseClass
{
    [LogCalls]
    public static void Foo()
    {
    }
}

internal class DerivedClass : BaseClass
{
}";
            const string TestCode = "DerivedClass.Foo();";
            const string ExpectedOutput = @"  TRACE|Enter BaseClass.Foo()
  TRACE|Leave BaseClass.Foo()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        #endregion
    }
}
