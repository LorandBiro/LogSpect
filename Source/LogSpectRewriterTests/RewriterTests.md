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



### InterfaceMethod

```C#
internal interface IFoo
{
    [LogCalls]
    void Bar();
}
```

Warnings:
```
System.Void IFoo::Bar() - LogCallsAttribute doesn't have any effect on interface members.
```




### InterfacePropertyGetter

```C#
internal interface IFoo
{
    int Bar { [LogCalls] get; set; }
}
```

Warnings:
```
System.Int32 IFoo::get_Bar() - LogCallsAttribute doesn't have any effect on interface members.
```





### InterfacePropertySetter

```C#
internal interface IFoo
{
    int Bar { get; [LogCalls] set; }
}
```

Warnings:
```
System.Void IFoo::set_Bar(System.Int32) - LogCallsAttribute doesn't have any effect on interface members.
```






### InterfaceIndexerGetter

```C#
internal interface IFoo
{
    int this[int index] { [LogCalls] get; set; }
}
```

Warnings:
```
System.Int32 IFoo::get_Item(System.Int32) - LogCallsAttribute doesn't have any effect on interface members.
```







### InterfaceIndexerSetter

```C#
internal interface IFoo
{
    int this[int index] { get; [LogCalls] set; }
}
```

Warnings:
```
System.Void IFoo::set_Item(System.Int32,System.Int32) - LogCallsAttribute doesn't have any effect on interface members.
```
