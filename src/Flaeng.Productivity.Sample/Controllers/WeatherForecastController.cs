using Flaeng.Productivity.Sample.Services;

using Microsoft.AspNetCore.Mvc;

namespace Flaeng.Productivity.Sample.Controllers;

[ApiController, Route("[controller]")]
public partial class WeatherForecastController : ControllerBase
{
    [Flaeng.Inject] private readonly ILogger<WeatherForecastController> _logger1;
    [Flaeng.Inject] protected readonly ILogger<WeatherForecastController> _logger2;
    [Flaeng.Inject] internal readonly ILogger<WeatherForecastController> _logger3;
    [Flaeng.Inject] public readonly ILogger<WeatherForecastController> _logger4;
    [Flaeng.Inject] readonly ILogger<WeatherForecastController> _logger5;
    [Flaeng.Inject] readonly ILogger<WeatherForecastController> _logger6;
    [Flaeng.Inject] ILogger<WeatherForecastController> _logger7 { get; }
    [Flaeng.Inject] IWeatherForecastService _weatherForecastService { get; }

    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult<List<WeatherForecast>> Get(CancellationToken token)
    {
        var list = _weatherForecastService.GetWeatherForecast(includeCity: true);
        return list.ToList();
    }
}
