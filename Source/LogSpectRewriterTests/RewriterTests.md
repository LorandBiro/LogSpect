### Constructor

```C#
public class Foo
{
    public static bool HasRun;

    [LogCalls]
    public Foo()
    {
        HasRun = true;
    }
}
```

```C#
Foo foo = new Foo();
Assert.IsTrue(Foo.HasRun);
```

Output:
```
  TRACE|Enter Foo..ctor()
  TRACE|Leave Foo..ctor()
```



### StaticConstructor

```C#
internal class Foo
{
    public static bool HasRun;

    [LogCalls]
    static Foo()
    {
        HasRun = true;
    }
}
```

```C#
Assert.IsTrue(Foo.HasRun);
```

Output:
```
  TRACE|Enter Foo..cctor()
  TRACE|Leave Foo..cctor()
```



### PropertyGetter

```C#
internal class Foo
{
    public int Bar { [LogCalls] get; set; }
}
```

```C#
Foo foo = new Foo();
foo.Bar = 3;
Assert.AreEqual(3, foo.Bar);
```

Output:
```
  TRACE|Enter Foo.get_Bar()
  TRACE|Leave Foo.get_Bar(): 3
```



### PropertySetter

```C#
internal class Foo
{
    public int Bar { get; [LogCalls] set; }
}
```

```C#
Foo foo = new Foo();
foo.Bar = 3;
Assert.AreEqual(3, foo.Bar);
```

Output:
```
  TRACE|Enter Foo.set_Bar(value: 3)
  TRACE|Leave Foo.set_Bar()
```



### MethodsInBaseClassShowsTheNameOfTheExecutingClass

```C#
internal abstract class BaseClass
{
    [LogCalls]
    public void Foo()
    {
    }
}

internal class DerivedClass : BaseClass
{
}
```

```C#
DerivedClass derived = new DerivedClass();
derived.Foo();
```

Output:
```
  TRACE|Enter DerivedClass.Foo()
  TRACE|Leave DerivedClass.Foo()
```



### StaticMethodsInBaseClassShowsTheNameOfTheBaseClass

```C#
internal abstract class BaseClass
{
    [LogCalls]
    public static void Foo()
    {
    }
}

internal class DerivedClass : BaseClass
{
}
```

```C#
DerivedClass.Foo();
```

Output:
```
  TRACE|Enter BaseClass.Foo()
  TRACE|Leave BaseClass.Foo()
```



### TryCatch

```C#
internal class Foo
{
    [LogCalls]
    public static void Bar()
    {
        try
        {
            Dummy();
        }
        catch (Exception)
        {
            Dummy();
        }
    }

    private static void Dummy()
    {
        // So the try-catch won't be optimized out by the compiler.
    }
}
```

```C#
Foo.Bar();
```

Output:
```
  TRACE|Enter Foo.Bar()
  TRACE|Leave Foo.Bar()
```



### TryFinally

```C#
internal class Foo
{
    [LogCalls]
    public static void Bar()
    {
        try
        {
            Dummy();
        }
        finally
        {
            Dummy();
        }
    }

    private static void Dummy()
    {
        // So the try-finally won't be optimized out by the compiler.
    }
}
```

```C#
Foo.Bar();
```

Output:
```
  TRACE|Enter Foo.Bar()
  TRACE|Leave Foo.Bar()
```
