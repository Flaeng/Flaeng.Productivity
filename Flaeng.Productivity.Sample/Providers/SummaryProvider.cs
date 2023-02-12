using Flaeng.Productivity.DependencyInjection;

namespace Flaeng.Productivity.Sample.Providers;

[GenerateInterface]// /*, Register*/]
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