# FluentApiGenerator

## Introduction

FluentApiGenerator generates extension methods for each property or field marked with the attribute MakeFluent (Flaeng.MakeFluentAttribute) or for all fields or properties in classes marked with the attribute. These methods can be used to chain-setting fields, in common called a FluentAPI.

## Example

### Input

```csharp
[Flaeng.MakeFluent]
public class Blah 
{ 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age;
}
```

### Output

```csharp
public static partial class BlahExtensions
{
    public static Blah FirstName(
        this Blah _this,
        global::System.String FirstName
    )
    {
        _this.FirstName = FirstName;
        return _this;
    }
    public static Blah LastName(
        this Blah _this,
        global::System.String LastName
    )
    {
        _this.LastName = LastName;
        return _this;
    }
    public static Blah Age(
        this Blah _this,
        global::System.Int32 Age
    )
    {
        _this.Age = Age;
        return _this;
    }
}
```

## Known issues

No known issues