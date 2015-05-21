namespace LogSpectRewriterTests
{
    using System;
    using LogSpectRewriterTests.Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FormattingIntegrationTests
    {
        [TestMethod]
        public void DoNotLogReturnValue()
        {
            const string ClassDefinitions = @"
public class Foo
{
    [LogCalls]
    [DoNotLog]
    public static int Bar()
    {
        return 42;
    }
}";
            const string TestCode = "Foo.Bar();";
            const string ExpectedOutput = @"  TRACE|Enter Foo.Bar()
  TRACE|Leave Foo.Bar(): -
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void DoNotLogParameter()
        {
            const string ClassDefinitions = @"
public class Foo
{
    [LogCalls]
    public static void Bar([DoNotLog] int p)
    {
    }
}";
            const string TestCode = "Foo.Bar(42);";
            const string ExpectedOutput = @"  TRACE|Enter Foo.Bar(p: -)
  TRACE|Leave Foo.Bar()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }

        [TestMethod]
        public void DoNotLogProperty()
        {
            const string ClassDefinitions = @"
public class Complex
{
    [DoNotLog]
    public int Re { get; set; }

    public int Im { get; set; }
}

public class Foo
{
    [LogCalls]
    public static void Bar([LogMembers] Complex p)
    {
    }
}";
            const string TestCode = "Foo.Bar(new Complex { Re = 1, Im = 2 });";
            const string ExpectedOutput = @"  TRACE|Enter Foo.Bar(p: { Re: -, Im: 2 })
  TRACE|Leave Foo.Bar()
";

            CodeRunner.CompileRewriteAndRun(ClassDefinitions, TestCode, ExpectedOutput);
        }
    }
}
