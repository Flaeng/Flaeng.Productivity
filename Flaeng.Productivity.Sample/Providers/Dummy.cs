using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace
{
    public partial class Dummy
    {
        [Inject] readonly IDictionary<string, object> _logger;
    }
}