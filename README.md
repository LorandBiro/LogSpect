LogSpect
========

Usually debugging is easy while you work on a software, but after you deploy a software to the production environment debugging becomes much harder. You lose the IDE, you lose control. You can't just put a breakpoint somewhere and investigate in the debugger, you have to rely on something else. If your software have a trace log you have a tool to start the investigation, but otherwise you'll have to guess and experiment. The more you log the better the chance it will contain something useful about the error and its circumstances.

LogSpect is a logging (or more accurately a tracing) framework. It doesn't try to replace the already popular frameworks (like NLog or log4net), but it works together with them to decrease the effort of logging by automatizing it. You mark your methods you need to log and LogSpect will use your favorite logging framework to log everything: the name of the method, the input and output parameters, the return value and even the exceptions. Additionally it will indent the log entries so it will be really easy to read.

# Quick tutorial

The fastest way to install the library is to use NuGet. For the sake of simplicity we won't use any other logging frameworks, only the included ConsoleLogger. So let's create a new Console Application, right click on References, select "Manage NuGet packages..." and type LogSpect in the search box. (Currently you have to change the "Stable only" dropbox to "Include Prerelease") Then install the "LogSpect" and "LogSpect.BasicLoggers" packages. The latter contains some basic loggers for simple scenarios, but connecting LogSpect with a mature logging framework like NLog or log4net is highly recommended.

By now everything is ready to use. A tool called LogSpectRewriter is included in the project's build process, but to see any actual logging we have to specify which methods to log and we have to tell LogSpect where to log them (NLog, log4net or one of the basic loggers). First let's write a few random methods, decorate them with the LogCallsAttribute and then initialize the `MethodLoggerFactory` with the chosen logger:

```C#
using System;
using LogSpect;
using LogSpect.BasicLoggers;

public class Program
{
    public static void Main(string[] args)
    {
        MethodLoggerFactory.SetFactory(new FormattingMethodLoggerFactory(new ColoredConsoleLoggerFactory()));
        Foo(10);
        Bar(5);
    }

    [LogCalls]
    public static void Foo(int x)
    {
        Bar(x * 2);
    }

    [LogCalls]
    public static int Bar(int y)
    {
        return y * y;
    }
}
```

Now if you start this application you'll see the following output on the console:

```
Enter Program.Foo(x: 10)
    Enter Program.Bar(y: 20)
    Leave Program.Bar(): 400
Leave Program.Foo()
Enter Program.Bar(y: 5)
Leave Program.Bar(): 25
```

As you can see the events like entering and leaving a method are logged with the parameters and return values. In the real life most of the methods are a bit more complicated than these, so let's see the following method:

```C#
[LogCalls]
public static void Foo(Complex a, [LogMembers] Complex b) 
{
}
```

By default LogSpect will use the parameters' `ToString` method for serialization, but for the second parameter we used the `LogMembersAttribute` which tells LogSpect to serialize the value's properties. If we call this method we'll see something like this:

```
Enter Program.Foo(a: (1, 2.5), b: { Real: 1, Imaginary: 2.5, Magnitude: 2.69258240356725, Phase: 1.19028994968253 })
Leave Program.Foo()
```

The `Complex` class's `ToString` method is pretty good but developers rarely implement this method for their own classes. For some objects it doesn't even make sense, but even if it has a good ToString implementation, you might need a more detailed description for your logs.

Also you often need to log the contents of a list or dictionary. By default LogSpect won't log their contents because they can be quite big, so you have to explicitly mark them with the `LogItemsAttribute`. Maybe you also want to log the properties of those items so you can combine this with the `LogMembersAttribute`. Let's see an example:

```C#
[LogCalls]
public static void Foo(int[] a, [LogItems] int[] b, [LogItems, LogMembers] Dictionary<string, Complex> c)
{
}
```

A possible output of this method is:

```
Enter Program.Foo(a: System.Int32[], b: [1, 2, 3], c: ["c" => { Real: 1, Imaginary: 2.5, Magnitude: 2.69258240356725, Phase: 1.19028994968253 }])
Leave Program.Foo()
```

You can see that LogSpect used the `ToString` method to serialize the first parameter. Important: `LogItemsAttribute` doesn't support IEnumerable, it can only be used on `ICollection` and `IDictionary` types, because enumerating an IEnumerable can cause bugs due to its possible side effects. Static code analyzers usually catch these problems, but in this case it's impossible to detect.

If want to control the serialization of the return values you can also use these attributes, but you have to decorate the return value and not the method:

```C#
[LogCalls]
[return: LogMembers]
public int[] Foo()
{
}
```

Still there are some more complicated scenarios, maybe you want to log a more complex, nested data structure. The `LogMembersAttribute` will tell LogSpect to serialize the properties of the parameter but it will use only their `ToString` implementation. To solve this you just have to use the same attributes on the properties, LogSpect will find them and use them for serialization. Let's say you have the following classes:

```C#
public class Bookmark
{
    public string Name { get; set; }

    public Uri Address { get; set; }
}

public class Browser
{
    public Uri HomePage { get; set; }

    [LogItems, LogMembers]
    public Bookmark[] Bookmarks { get; set; }
}
```

If you simply pass a `Browser` instance as a parameter LogSpect will use its `ToString` method for serialization, but if you decorate the parameter with `LogMembersAttribute` the output will be similar to this:

```
Enter Program.Foo(browser: { HomePage: http://github.com/, Bookmarks: [{ Name: "StackOverflow", Address: http://stackoverflow.com/ }] })
Leave Program.Foo()
```

A more detailed documentation will be coming soon...
