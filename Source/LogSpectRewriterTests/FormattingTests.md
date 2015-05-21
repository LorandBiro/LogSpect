### DoNotLogReturnValue

Code:
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

Run:
```C#
Foo.Bar();
```

Output:
```
  TRACE|Enter Foo.Bar()
  TRACE|Leave Foo.Bar(): -
```



### DoNotLogParameter

Code:
```C#
public class Foo
{
    [LogCalls]
    public static void Bar([DoNotLog] int p)
    {
    }
}
```

Run:
```C#
Foo.Bar(42);
```

Output:
```
  TRACE|Enter Foo.Bar(p: -)
  TRACE|Leave Foo.Bar()
```



### DoNotLogProperty

Code:
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

Run:
```C#
Foo.Bar(new Complex { Re = 1, Im = 2 });
```

Output:
```
  TRACE|Enter Foo.Bar(p: { Re: -, Im: 2 })
  TRACE|Leave Foo.Bar()
```
