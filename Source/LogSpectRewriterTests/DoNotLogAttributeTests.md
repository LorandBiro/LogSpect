### ReturnValue

```C#
public class Foo
{
    [LogCalls]
    [DoNotLog]
    public static int Bar()
    {
        return 42;
    }
}
```

```C#
Foo.Bar();
```

Output:
```
  TRACE|Enter Foo.Bar()
  TRACE|Leave Foo.Bar(): -
```



### Parameter

```C#
public class Foo
{
    [LogCalls]
    public static void Bar([DoNotLog] int p)
    {
    }
}
```

```C#
Foo.Bar(42);
```

Output:
```
  TRACE|Enter Foo.Bar(p: -)
  TRACE|Leave Foo.Bar()
```



### Property

```C#
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
}
```

```C#
Foo.Bar(new Complex { Re = 1, Im = 2 });
```

Output:
```
  TRACE|Enter Foo.Bar(p: { Re: -, Im: 2 })
  TRACE|Leave Foo.Bar()
```



### OnMethodWithoutLogCallsAttribute

```C#
internal class Foo
{
    [DoNotLog]
    public int Bar()
    {
        return 0;
    }
}
```

Warnings:
```
System.Int32 Foo::Bar() - DoNotLogAttribute doesn't have any effect on methods without LogCallsAttribute.
```



### OnParameterWithoutLogCallsAttribute

```C#
internal class Foo
{
    public void Bar([DoNotLog] int a)
    {
    }
}
```

Warnings:
```
System.Void Foo::Bar(System.Int32) - DoNotLogAttribute doesn't have any effect on methods without LogCallsAttribute.
```
