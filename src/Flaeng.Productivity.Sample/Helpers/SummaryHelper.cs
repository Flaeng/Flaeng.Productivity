namespace Flaeng.Productivity.Sample.Helpers;

public interface ISummaryHelper
{
    IEnumerable<WeatherForecast> GetData();
}
public class SummaryHelper : ISummaryHelper
{
    public IEnumerable<WeatherForecast> GetData() => new WeatherForecast[0];
}
