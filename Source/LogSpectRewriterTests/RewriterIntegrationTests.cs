namespace LogSpectRewriterTests
{
    using System;
    using LogSpect;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RewriterIntegrationTests
    {
        [TestMethod]
        public void Constructor()
        {
            const string ClassDefinitions = @"using LogSpect;

public class Foo
{
    [LogCalls]
    public Foo()
    {
    }
}";
            const string TestCode = "new Foo();";
            const string ExpectedOutput = @"  TRACE|Enter Foo..ctor()
  TRACE|Leave Foo..ctor()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void StaticConstructor()
        {
            const string ClassDefinitions = @"using LogSpect;

internal class Foo
{
    public static int Bar { get; set; }

    [LogCalls]
    static Foo()
    {
    }
}";
            const string TestCode = "Foo.Bar = 0;";
            const string ExpectedOutput = @"  TRACE|Enter Foo..cctor()
  TRACE|Leave Foo..cctor()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void PropertyGetter()
        {
            const string ClassDefinitions = @"using LogSpect;

internal class Foo
{
    public int Bar { [LogCalls] get; set; }
}";
            const string TestCode = "int bar = new Foo().Bar;";
            const string ExpectedOutput = @"  TRACE|Enter Foo.get_Bar()
  TRACE|Leave Foo.get_Bar(): 0
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void PropertySetter()
        {
            const string ClassDefinitions = @"using LogSpect;

internal class Foo
{
    public int Bar { get; [LogCalls] set; }
}";
            const string TestCode = "new Foo().Bar = 3;";
            const string ExpectedOutput = @"  TRACE|Enter Foo.set_Bar(value: 3)
  TRACE|Leave Foo.set_Bar()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void MethodsInBaseClassShowsTheNameOfTheDerivedClass()
        {
            const string ClassDefinitions = @"using LogSpect;

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
            const string ClassDefinitions = @"using LogSpect;

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
    }
}
