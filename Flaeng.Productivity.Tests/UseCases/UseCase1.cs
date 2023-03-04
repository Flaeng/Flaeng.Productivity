namespace Flaeng.Productivity.Tests.UseCases;

public class UseCase1 : BaseUseCase
{
    public UseCase1() : base("UseCase1") { }

    [Fact]
    public void Has_no_errors_or_warnings()
    {
        Assert.Empty(Result.Diagnostic);
    }

    [Fact]
    public void Can_generate_SummaryProvider_interface()
    {
        var generatedSource = Result.GeneratedFiles.SingleOrDefault(x => x.Filename.EndsWith(".ISummaryProvider.g.cs"));
        Assert.NotNull(generatedSource);

        Assert.Equal("""
        // <auto-generated/>

        using Flaeng.Productivity.DependencyInjection;
        
        #nullable enable

        namespace Flaeng.Productivity.Sample.Providers
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.0.0")]
            public interface ISummaryProvider
            {
                string[] GetSummaries();
            }
            public partial class SummaryProvider : ISummaryProvider
            {
            }
        }

        """, generatedSource.Content);
    }

    [Fact]
    public void Can_generate_WeatherForecastController_constructor()
    {
        var generatedSource = Result.GeneratedFiles.SingleOrDefault(x => x.Filename.EndsWith(".WeatherForecastController.g.cs"));
        Assert.NotNull(generatedSource);

        Assert.Equal("""
        // <auto-generated/>

        using Flaeng.Productivity.DependencyInjection;
        using Flaeng.Productivity.Sample.Services;
        using Microsoft.AspNetCore.Mvc;

        #nullable enable

        namespace Flaeng.Productivity.Sample.Controllers
        {
            public partial class WeatherForecastController
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.0.0")]
                public WeatherForecastController(
                    ILogger<WeatherForecastController> _logger1,
                    ILogger<WeatherForecastController> _logger2,
                    ILogger<WeatherForecastController> _logger3,
                    ILogger<WeatherForecastController> _logger4,
                    ILogger<WeatherForecastController> _logger5,
                    ILogger<WeatherForecastController> _logger6,
                    ILogger<WeatherForecastController> _logger7,
                    IWeatherForecastService _weatherForecastService
                    )
                {
                    this._logger1 = _logger1;
                    this._logger2 = _logger2;
                    this._logger3 = _logger3;
                    this._logger4 = _logger4;
                    this._logger5 = _logger5;
                    this._logger6 = _logger6;
                    this._logger7 = _logger7;
                    this._weatherForecastService = _weatherForecastService;
                }
            }
        }

        """, generatedSource.Content);
    }

    [Fact]
    public void Can_generate_WeatherForecastService_interface()
    {
        var generatedSource = Result.GeneratedFiles.SingleOrDefault(x => x.Filename.EndsWith(".IWeatherForecastService.g.cs"));
        Assert.NotNull(generatedSource);

        Assert.Equal("""
        // <auto-generated/>

        using Flaeng.Productivity.DependencyInjection;
        using Flaeng.Productivity.Sample.Providers;

        #nullable enable

        namespace Flaeng.Productivity.Sample.Services
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.0.0")]
            public interface IWeatherForecastService
            {
                IEnumerable<WeatherForecast> GetWeatherForecast();
            }
            public partial class WeatherForecastService : IWeatherForecastService
            {
            }
        }

        """, generatedSource.Content);
    }

    [Fact]
    public void Can_generate_WeatherForecastService_constructor()
    {
        var generatedSource = Result.GeneratedFiles.SingleOrDefault(x => x.Filename.EndsWith(".WeatherForecastService.g.cs"));
        Assert.NotNull(generatedSource);

        Assert.Equal("""
        // <auto-generated/>

        using Flaeng.Productivity.DependencyInjection;
        using Flaeng.Productivity.Sample.Providers;

        #nullable enable

        namespace Flaeng.Productivity.Sample.Services
        {
            public partial class WeatherForecastService
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "0.2.0.0")]
                public WeatherForecastService(
                    ISummaryProvider _summaryProvider
                    )
                {
                    this._summaryProvider = _summaryProvider;
                }
            }
        }

        """, generatedSource.Content);
    }

}
