# Flaeng.Productivity

[![CodeFactor](https://www.codefactor.io/repository/github/flaeng/flaeng.productivity/badge/main)](https://www.codefactor.io/repository/github/flaeng/flaeng.productivity/overview/main)
[![Maintainability](https://api.codeclimate.com/v1/badges/59770f285df113dc53c7/maintainability)](https://codeclimate.com/github/Flaeng/Flaeng.Productivity/maintainability)

![Nuget](https://img.shields.io/nuget/v/Flaeng.Productivity)
![Nuget](https://img.shields.io/nuget/dt/Flaeng.Productivity)


## Mission / What does it do

Flaeng.Productivity aims to improve productivity and developer experience by generating code, that would often be boilerplate, using C# source generators

## How does it that?

GenerateInterfaceAttribute generates a interface with all the public properties, fields and methods in your class so you don't have to keep it up-to-date but only need to change your test code.

InjectAttribute generates a constructor for the declaring class with parameters for every property or field with the InjectAttribute in the order they are defined in the class.

MakeFluentAttribute - Makes methods for each property or field marked with the attribute (or all properties and fields if marked on the class) so that you can chain the method-calls like a fluent API

RegisterServiceAttribute - Makes an RegisterServices-extensions-method on the IServiceCollection-interface so that you don't have to remmeber to add your services to you dependency injection container

## Examples

[More examples in the docs](https://github.com/Flaeng/Flaeng.Productivity/blob/main/docs/README.md)

### GenerateInterface Example 1: Input
```csharp
using System;

namespace Flaeng.Productivity.Sample.Providers;

[Flaeng.GenerateInterface]
public partial class SummaryProvider
{
    public string[] GetSummaries()
    {
        return new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
    }
}
```
### GenerateInterface Example 1: Output
```csharp
// <auto-generated/>

using System;

#nullable enable

namespace Flaeng.Productivity.Sample.Providers
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.3.0")]
    public interface ISummaryProvider
    {
        string[] GetSummaries();
    }
    public partial class SummaryProvider : ISummaryProvider
    {
    }
}

```

### GenerateInterface and Inject Example 2: Input
```csharp
using Flaeng.Productivity.Sample.Providers;

namespace Flaeng.Productivity.Sample.Services;

[Flaeng.GenerateInterface]
public partial class WeatherForecastService
{
    [Flaeng.Inject] protected readonly ISummaryProvider _summaryProvider;

    public IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        var Summaries = _summaryProvider.GetSummaries();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
```
### GenerateInterface and Inject Example 2: Output
```csharp
// <auto-generated/>

using Flaeng.Productivity.DependencyInjection;
using Flaeng.Productivity.Sample.Providers;

#nullable enable

namespace Flaeng.Productivity.Sample.Services
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.3.0")]
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> GetWeatherForecast();
    }
    public partial class WeatherForecastService : IWeatherForecastService
    {
    }
}

```

```csharp
// <auto-generated/>

using Flaeng.Productivity.DependencyInjection;
using Flaeng.Productivity.Sample.Providers;

#nullable enable

namespace Flaeng.Productivity.Sample.Services
{
    public partial class WeatherForecastService
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.3.0")]
        public WeatherForecastService(
            ISummaryProvider _summaryProvider
            )
        {
            this._summaryProvider = _summaryProvider;
        }
    }
}
```

### MakeFluent Example 1: Input
```csharp
[Flaeng.MakeFluent]
public class Blah 
{ 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age;
}
```

### MakeFluent Example 1: Output
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


### RegisterService Example 1: Input
```csharp
namespace TestNamespace
{
    public interface IMyServiceInterface { }
    [Flaeng.RegisterService]
    public class MyService : IMyServiceInterface { }

    public interface IMyTransientServiceInterface { }
    [Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Transient)]
    public class MyTransientService : IMyTransientServiceInterface { }

    public interface IMyScopedServiceInterface { }
    [Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Scoped)]
    public class MyScopedService : IMyScopedServiceInterface { }

    public interface IMySingletonServiceInterface { }
    [Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Singleton)]
    public class MySingletonService : IMySingletonServiceInterface { }
}
```

### RegisterService Example 1: Output
```csharp
using Microsoft.Extensions.DependencyInjection;

public static partial class StartupExtensions
{
    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection RegisterServices(
        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services
    )
    {
        services.AddScoped<global::TestNamespace.IMyServiceInterface, global::TestNamespace.MyService>();
        services.AddTransient<global::TestNamespace.IMyTransientServiceInterface, global::TestNamespace.MyTransientService>();
        services.AddScoped<global::TestNamespace.IMyScopedServiceInterface, global::TestNamespace.MyScopedService>();
        services.AddSingleton<global::TestNamespace.IMySingletonServiceInterface, global::TestNamespace.MySingletonService>();
        return services;
    }
}
```

[More examples in the docs](https://github.com/Flaeng/Flaeng.Productivity/blob/main/docs/README.md)
