namespace Flaeng.Productivity.Sample.Providers;

public partial interface ISummaryProvider { }

[Flaeng.RegisterService(ServiceType = ServiceType.Scoped)]
[Flaeng.GenerateInterface]
public partial class SummaryProvider : ISummaryProvider
{
    public string[] GetSummaries()
    {
        return new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
    }
}
