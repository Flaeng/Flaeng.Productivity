namespace Flaeng.Productivity.Sample;

public static class WeatherForecastMaker
{
    public static WeatherForecast Create()
    {
        return new WeatherForecast()
            .Date(new DateOnly(2023, 7, 29))
            .TemperatureC(31)
            .Summary("Sunny");
    }
}
