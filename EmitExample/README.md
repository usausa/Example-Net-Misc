# Dynamic code generation study example.

## IActivator

```csharp
public interface IActivator
{
    ConstructorInfo Source { get; }

    object Create(params object[] arguments);
}
```

## IAccessor

```csharp
public interface IAccessor
{
    PropertyInfo Source { get; }

    object GetValue(object target);

    void SetValue(object target, object value);
}
```
