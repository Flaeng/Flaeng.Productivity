namespace Flaeng.Productivity.Tests;

public class CompleteUseCaseTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public CompleteUseCaseTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    readonly IIncrementalGenerator[] generators = new IIncrementalGenerator[]
    {
        new ConstructorGenerator(),
        new StartupGenerator(),
        new InterfaceGenerator(),
        new FluentApiGenerator()
    };

    [Fact(Timeout = 1000)]
    public void InterfaceGenerator_and_ConstructorGenerator_together()
    {
        // Given
        var modelSource = new SourceFile("Models/WeatherForecast.cs", """
        using System;
        
        #nullable enable

        namespace Flaeng.Productivity.Sample.Models;

        [Flaeng.MakeFluent]
        public class WeatherForecast
        {
            public DateOnly Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

            public string? Summary { get; set; }
        }
        """);
        var providerSource = new SourceFile("Providers/SummaryProvider.cs", """
        namespace Flaeng.Productivity.Sample.Providers;

        #nullable enable

        [Flaeng.GenerateInterface]
        [Flaeng.RegisterService(ServiceType = ServiceType.Scoped)]
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
        """);
        var serviceSource = new SourceFile("Services/WeatherForecastService.cs", """
        using System;
        using System.Linq;
        using System.Collections.Generic;

        using Flaeng.Productivity.Sample.Models;
        using Flaeng.Productivity.Sample.Providers;

        #nullable disable

        namespace Flaeng.Productivity.Sample.Services
        {
            [Flaeng.GenerateInterface, Flaeng.RegisterService]
            public partial class WeatherForecastService
            {
                [Flaeng.Inject] protected readonly ISummaryProvider _summaryProvider;

                public IEnumerable<WeatherForecast> GetWeatherForecast()
                {
                    var Summaries = _summaryProvider.GetSummaries();
                    var now = DateTime.Now;
                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                    .ToArray();
                }

                protected void Test() { }

                private WeatherForecast Map() => new();
            }
        }
        """);
        var controllerSource = new SourceFile("Controllers/WeatherForecastController.cs", """
        using System.Linq;
        using System.Threading;
        using System.Collections.Generic;

        using Flaeng.Productivity.Sample.Services;
        using Flaeng.Productivity.Sample.Models;

        #nullable disable
        #pragma warning disable CS0169
        #pragma warning disable CS0649

        namespace Flaeng.Productivity.Sample.Controllers;

        //[ApiController, Route("[controller]")]
        public partial class WeatherForecastController
        {
            [Flaeng.Inject] private readonly IList<WeatherForecastController> _logger1;
            [Flaeng.Inject] protected readonly IList<WeatherForecastController> _logger2;
            [Flaeng.Inject] internal readonly IList<WeatherForecastController> _logger3;
            [Flaeng.Inject] public readonly IList<WeatherForecastController> _logger4;
            [Flaeng.Inject] readonly IList<WeatherForecastController> _logger5;
            [Flaeng.Inject] readonly IList<WeatherForecastController> _logger6;
            [Flaeng.Inject] IList<WeatherForecastController> _logger7 { get; }

            [Flaeng.Inject] IWeatherForecastService _weatherForecastService { get; }

            //[HttpGet(Name = "GetWeatherForecast")]
            public List<WeatherForecast> Get(CancellationToken token)
            {
                var list = _weatherForecastService.GetWeatherForecast();
                return list.ToList();
            }
        }
        """);

        // When
        var compilcation = compiler.GetGeneratedOutput(
            generators,
            new[]
            {
                modelSource,
                providerSource,
                serviceSource,
                controllerSource
            }
        );

        // Then
        var errors = compilcation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Error);
        Assert.Empty(errors);
        Assert.Empty(compilcation.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void InterfaceGenerator_and_ConstructorGenerator_together_alternate()
    {
        // Given
        var modelSource = new SourceFile("Models/WeatherForecast.cs", """
        using System;
        
        #nullable enable

        namespace Flaeng.Productivity.Sample.Models;

        public class WeatherForecast
        {
            public DateOnly Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

            public string? Summary { get; set; }
        }
        """);
        var providerSource = new SourceFile("Providers/SummaryProvider.cs", """
        namespace Flaeng.Productivity.Sample.Providers;

        #nullable enable

        [Flaeng.GenerateInterface]
        [Flaeng.RegisterService(ServiceType = ServiceType.Scoped)]
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
        """);
        var serviceSource = new SourceFile("Services/WeatherForecastService.cs", """
        using System;
        using System.Linq;
        using System.Collections.Generic;

        using Flaeng.Productivity.Sample.Models;
        using Flaeng.Productivity.Sample.Providers;

        #nullable disable

        namespace Flaeng.Productivity.Sample.Services;

        [Flaeng.GenerateInterface, Flaeng.RegisterService]
        public partial class WeatherForecastService
        {
            [Flaeng.Inject] protected readonly ISummaryProvider _summaryProvider;

            public IEnumerable<WeatherForecast> GetWeatherForecast()
            {
                var Summaries = _summaryProvider.GetSummaries();
                var now = DateTime.Now;
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
            }

            protected void Test() { }

            private WeatherForecast Map() => new();
        }
        """);
        var controllerSource = new SourceFile("Controllers/WeatherForecastController.cs", """
        using System.Linq;
        using System.Threading;
        using System.Collections.Generic;

        using Flaeng.Productivity.Sample.Services;
        using Flaeng.Productivity.Sample.Models;

        #nullable disable
        #pragma warning disable CS0169
        #pragma warning disable CS0649

        namespace Flaeng.Productivity.Sample.Controllers;

        //[ApiController, Route("[controller]")]
        public partial class WeatherForecastController
        {
            [Flaeng.Inject] private readonly IList<WeatherForecastController> _logger1;
            [Flaeng.Inject] protected readonly IList<WeatherForecastController> _logger2;
            [Flaeng.Inject] internal readonly IList<WeatherForecastController> _logger3;
            [Flaeng.Inject] public readonly IList<WeatherForecastController> _logger4;
            [Flaeng.Inject] readonly IList<WeatherForecastController> _logger5;
            [Flaeng.Inject] readonly IList<WeatherForecastController> _logger6;
            [Flaeng.Inject] IList<WeatherForecastController> _logger7 { get; }

            [Flaeng.Inject] IWeatherForecastService _weatherForecastService { get; }

            //[HttpGet(Name = "GetWeatherForecast")]
            public List<WeatherForecast> Get(CancellationToken token)
            {
                var list = _weatherForecastService.GetWeatherForecast();
                return list.ToList();
            }
        }
        """);

        // When
        var compilcation = compiler.GetGeneratedOutput(
            generators.Reverse().ToArray(),
            new[]
            {
                modelSource,
                providerSource,
                serviceSource,
                controllerSource
            }
        );

        // Then
        var errors = compilcation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Error);
        Assert.Empty(errors);
        Assert.Empty(compilcation.Diagnostic);
    }
}
