### OnMethodWithoutLogCallsAttribute

```C#
internal class Foo
{
    [LogItems]
    public int[] Bar()
    {
        return null;
    }
}
```

Warnings:
```
System.Int32[] Foo::Bar() - LogItemsAttribute doesn't have any effect on methods without LogCallsAttribute.
```



### OnParameterWithoutLogCallsAttribute

```C#
internal class Foo
{
    public void Bar([LogItems] int[] a)
    {
    }
}
```

Warnings:
```
System.Void Foo::Bar(System.Int32[]) - LogItemsAttribute doesn't have any effect on methods without LogCallsAttribute.
```



### IEnumerableParameter

```C#
internal class Foo
{
    [LogCalls]
    public static void Bar([LogItems] IEnumerable<int> a)
    {
    }
}
```

Warnings:
```
System.Void Foo::Bar(System.Collections.Generic.IEnumerable`1<System.Int32>) - LogItemsAttribute will work only on ICollection and IDictionary values.
```

```C#
Foo.Bar(new[] { 1, 2, 3 });
Foo.Bar(new[] { 1, 2, 3 }.Select(x => x * 2));
```

Output:
```
  TRACE|Enter Foo.Bar(a: [1, 2, 3])
  TRACE|Leave Foo.Bar()
  TRACE|Enter Foo.Bar(a: System.Linq.Enumerable+WhereSelectArrayIterator`2[System.Int32,System.Int32])
  TRACE|Leave Foo.Bar()
```


### IEnumerableMethod

```C#
internal class Foo
{
    [LogCalls]
    [LogItems]
    public static IEnumerable<int> Bar(bool linq)
    {
        if (linq)
        {
            return new[] { 1, 2, 3 }.Select(x => x * 2);
        }
        else
        {
            return new[] { 1, 2, 3 };
        }
    }
}
```

Warnings:
```
System.Collections.Generic.IEnumerable`1<System.Int32> Foo::Bar(System.Boolean) - LogItemsAttribute will work only on ICollection and IDictionary values.
```

```C#
IEnumerable<int> a = Foo.Bar(false);
IEnumerable<int> b = Foo.Bar(true);
```

Output:
```
  TRACE|Enter Foo.Bar(linq: False)
  TRACE|Leave Foo.Bar(): [1, 2, 3]
  TRACE|Enter Foo.Bar(linq: True)
  TRACE|Leave Foo.Bar(): System.Linq.Enumerable+WhereSelectArrayIterator`2[System.Int32,System.Int32]
```



### IEnumerableProperty

```C#
internal class Foo
{
    [LogItems]
    public IEnumerable<int> Bar { get; set; }
}

internal class Lorem
{
    [LogCalls]
    public static void Ipsum([LogMembers] Foo foo)
    {
    }
}
```

Warnings:
```
System.Collections.Generic.IEnumerable`1<System.Int32> Foo::Bar() - LogItemsAttribute will work only on ICollection and IDictionary values.
```

```C#
Lorem.Ipsum(new Foo { Bar = new[] { 1, 2, 3 }});
Lorem.Ipsum(new Foo { Bar = new[] { 1, 2, 3 }.Select(x => x * 2)});
```

Output:
```
  TRACE|Enter Lorem.Ipsum(foo: { Bar: [1, 2, 3] })
  TRACE|Leave Lorem.Ipsum()
  TRACE|Enter Lorem.Ipsum(foo: { Bar: System.Linq.Enumerable+WhereSelectArrayIterator`2[System.Int32,System.Int32] })
  TRACE|Leave Lorem.Ipsum()
```
