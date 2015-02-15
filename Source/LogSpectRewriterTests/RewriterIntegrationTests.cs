namespace LogSpectRewriterTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RewriterIntegrationTests
    {
        [TestMethod]
        public void MethodsInBaseClassShowsTheNameOfTheDerivedClass()
        {
            const string Source = @"using LogSpect;

internal abstract class BaseClass
{
    [LogCalls]
    public void Foo()
    {
    }
}

internal class DerivedClassA : BaseClass { }
internal class DerivedClassB : BaseClass { }

public static class InheritenceTest
{
    public static void Run()
    {
        DerivedClassA a = new DerivedClassA();
        DerivedClassB b = new DerivedClassB();
        a.Foo();
        b.Foo();
    }
}";

            const string ExpectedOutput = @"  TRACE|Enter DerivedClassA.Foo()
  TRACE|Leave DerivedClassA.Foo()
  TRACE|Enter DerivedClassB.Foo()
  TRACE|Leave DerivedClassB.Foo()
";

            CodeRunner.CompileRewriteAndRun(Source, ExpectedOutput);
        }

        [TestMethod]
        public void StaticMethodsInBaseClassShowsTheNameOfTheBaseClass()
        {
            const string Source = @"using LogSpect;

internal abstract class BaseClass
{
    [LogCalls]
    public static void Foo()
    {
    }
}

internal class DerivedClass : BaseClass { }

public static class InheritenceTest
{
    public static void Run()
    {
        DerivedClass.Foo();
    }
}";

            const string ExpectedOutput = @"  TRACE|Enter BaseClass.Foo()
  TRACE|Leave BaseClass.Foo()
";

            CodeRunner.CompileRewriteAndRun(Source, ExpectedOutput);
        }
    }
}
