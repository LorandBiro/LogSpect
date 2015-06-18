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



### AbstractMethod

```C#
internal abstract class Foo
{
    [LogCalls]
    public abstract void Bar();
}
```

Warnings:
```
System.Void Foo::Bar() - LogCallsAttribute doesn't have any effect on abstract members.
```



### AbstractPropertyGetter

```C#
internal abstract class Foo
{
    public abstract int Bar { [LogCalls] get; set; }
}
```

Warnings:
```
System.Int32 Foo::get_Bar() - LogCallsAttribute doesn't have any effect on abstract members.
```



### AbstractPropertySetter

```C#
internal abstract class Foo
{
    public abstract int Bar { get; [LogCalls] set; }
}
```

Warnings:
```
System.Void Foo::set_Bar(System.Int32) - LogCallsAttribute doesn't have any effect on abstract members.
```



### AbstractIndexerGetter

```C#
internal abstract class Foo
{
    public abstract int this[int index] { [LogCalls] get; set; }
}
```

Warnings:
```
System.Int32 Foo::get_Item(System.Int32) - LogCallsAttribute doesn't have any effect on abstract members.
```



### AbstractIndexerSetter

```C#
internal abstract class Foo
{
    public abstract int this[int index] { get; [LogCalls] set; }
}
```

Warnings:
```
System.Void Foo::set_Item(System.Int32,System.Int32) - LogCallsAttribute doesn't have any effect on abstract members.
```
