using Flaeng.Productivity.DependencyInjection;
using Flaeng.Productivity.Sample.Services;

using Microsoft.AspNetCore.Mvc;

namespace Flaeng.Productivity.Sample.Controllers;

[ApiController, Route("[controller]")]
public partial class WeatherForecastController : ControllerBase
{
    [Inject] private readonly ILogger<WeatherForecastController> _logger1;
    [Inject] protected readonly ILogger<WeatherForecastController> _logger2;
    [Inject] internal readonly ILogger<WeatherForecastController> _logger3;
    [Inject] public readonly ILogger<WeatherForecastController> _logger4;
    [Inject] readonly ILogger<WeatherForecastController> _logger5;
    [Inject] readonly ILogger<WeatherForecastController> _logger6;
    [Inject] ILogger<WeatherForecastController> _logger7 { get; }

    [Inject] IWeatherForecastService _weatherForecastService { get; }

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get(CancellationToken token)
    {
        var list = _weatherForecastService.GetWeatherForecast();
        return Ok(list);
        // return Ok();
    }
}