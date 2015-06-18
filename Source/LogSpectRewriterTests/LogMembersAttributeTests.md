### OnMethodWithoutLogCallsAttribute

```C#
internal class Foo
{
    [LogMembers]
    public string Bar()
    {
        return null;
    }
}
```

Warnings:
```
System.String Foo::Bar() - LogMembersAttribute doesn't have any effect on methods without LogCallsAttribute.
```



### OnParameterWithoutLogCallsAttribute

```C#
internal class Foo
{
    public void Bar([LogMembers] string a)
    {
    }
}
```

Warnings:
```
System.Void Foo::Bar(System.String) - LogMembersAttribute doesn't have any effect on methods without LogCallsAttribute.
```
