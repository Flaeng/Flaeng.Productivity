# ConstructorGenerator

## Introduction

ConstructorGenerator generates a constructor for each class containing a field or property with the attribute Inject (Flaeng.InjectAttribute).
The generated constructor accepts all fields and properties that have this attribute and sets them to the correct field.

Important: The class needs to be *partial* for the generator to extend the class.

## Example

### Input

```csharp
using Flaeng.Productivity.Sample.Providers;

namespace Flaeng.Productivity.Sample.Services;

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

### Output

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

## Known issues

CSharp doesnt know that the generator will se the field or property and will sometimes show the warning that the field/property is not being set.