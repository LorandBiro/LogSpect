### Constructor

Code:
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

Run:
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

Code:
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

Run:
```C#
Assert.IsTrue(Foo.HasRun);
```

Output:
```
  TRACE|Enter Foo..cctor()
  TRACE|Leave Foo..cctor()
```



### PropertyGetter

Code:
```C#
internal class Foo
{
    public int Bar { [LogCalls] get; set; }
}
```

Run:
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

Code:
```C#
internal class Foo
{
    public int Bar { get; [LogCalls] set; }
}
```

Run:
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

Code:
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

Run:
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

Code:
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

Run:
```C#
DerivedClass.Foo();
```

Output:
```
  TRACE|Enter BaseClass.Foo()
  TRACE|Leave BaseClass.Foo()
```
