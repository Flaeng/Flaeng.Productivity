using Flaeng.Productivity.Sample.Providers;

namespace Flaeng.Productivity.Sample.Services;

[Flaeng.GenerateInterface, Flaeng.RegisterService]
public partial class WeatherForecastService
{
    [Flaeng.Inject] protected readonly ISummaryProvider _summaryProvider;

    public IEnumerable<WeatherForecast> GetWeatherForecast(bool includeCity)
    {
        var Summaries = _summaryProvider.GetSummaries();
        var now = DateTime.Now;
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = Random.Shared.Next(-20, 55),
            City = "Silkeborg",
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .If(includeCity == false, RemoveCity)
        .ToArray();
    }

    private IEnumerable<WeatherForecast> RemoveCity(IEnumerable<WeatherForecast> enumerable)
    {
        foreach (var item in enumerable)
        {
            item.City = null;
            yield return item;
        }
    }

    protected void Test() { }

    private WeatherForecast Map() => new();
}
