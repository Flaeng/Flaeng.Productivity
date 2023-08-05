# StartupGenerator

## Introduction

StartupGenerator generates an static class (extension class) for the [IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) that you can call. This method will register all services to the IServiceCollection that has RegisterService (Flaeng.RegisterServiceAttribute).

## Example

### Example 1 - Simple add service examples - Input

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

### Example 1 - Simple add service examples - Output

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

### Example 2 - Custom method name - Input

```csharp
namespace TestNamespace
{
    [assembly: Flaeng.StartupExtension(MethodName = "ThisIsATest")]

    public interface IMyServiceInterface { }
    [Flaeng.RegisterService]
    public class MyService : IMyServiceInterface { }
}
```

### Example 2 - Custom method name - Output

```csharp
using Microsoft.Extensions.DependencyInjection;

public static partial class StartupExtensions
{
    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection ThisIsATest(
        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services
    )
    {
        services.AddScoped<global::TestNamespace.IMyServiceInterface, global::TestNamespace.MyService>();
        return services;
    }
}
```

## Known issues

The generator will always a public method called RegisterServices. This can give conflict if used with multiple projects or cross-projects.