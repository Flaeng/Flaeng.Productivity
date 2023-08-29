using System.IO;

namespace Flaeng.Productivity.CodeAnalyzer;

interface IRule
{
    void Configure(IImmutableDictionary<string, string> settings);
    bool AppliesTo(object node);
    Task<List<Diagnostic>> Validate(object node, CancellationToken token);
}

abstract class RuleBase<T> : IRule where T : class
{
    public abstract void Configure(IImmutableDictionary<string, string> settings);
    protected virtual bool AppliesTo(SyntaxNode node) => node is T;
    public bool AppliesTo(object obj) => obj is T item && AppliesTo(item);
    protected abstract Task<List<Diagnostic>> Validate(T node, CancellationToken token);
    public Task<List<Diagnostic>> Validate(object node, CancellationToken token)
    {
        var item = Unsafe.As<T>(node);
        return Validate(item, token);
    }
}

abstract class SyntaxNodeRuleBase<T> : IRule where T : SyntaxNode
{
    public abstract void Configure(IImmutableDictionary<string, string> settings);
    public virtual bool AppliesTo(SyntaxNode node) => node is T;
    public bool AppliesTo(object obj) => obj is T item && AppliesTo(item);
    protected abstract Task<List<Diagnostic>> Validate(T node, CancellationToken token);
    public Task<List<Diagnostic>> Validate(object node, CancellationToken token)
    {
        var item = Unsafe.As<T>(node);
        return Validate(item, token);
    }
}

class FileLengthRule : RuleBase<SyntaxTree>
{
    public const string SettingKey = "flaeng.filelength";
    private DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
    private int Limit = 250;

    public override void Configure(IImmutableDictionary<string, string> settings)
    {
        string key;
        if (settings.TryGetKey($"{SettingKey}.severity", out key))
            Severity = (DiagnosticSeverity)Enum.Parse(typeof(DiagnosticSeverity), settings[key]);

        if (settings.TryGetKey($"{SettingKey}.limit", out key))
            Limit = int.Parse(settings[key]);
    }

    protected override async Task<List<Diagnostic>> Validate(SyntaxTree tree, CancellationToken token)
    {
        var text = await tree.GetTextAsync(token);
        if (Limit <= text.Lines.Count)
            return new List<Diagnostic>();

        var desc = new DiagnosticDescriptor(
            id: "",
            title: "",
            messageFormat: "",
            category: "",
            Severity,
            isEnabledByDefault: true,
            description: ""
        );
        var diagnostic = Diagnostic.Create(desc, location: null);
        return new List<Diagnostic> { diagnostic };
    }
}

class SyntaxNodeSourceLength
{
    public async Task<int> getLineCount(SyntaxNode node, CancellationToken token)
    {
        var source = await Task.Factory.StartNew(() => node.GetText(), token);
        return source.Lines.Count;
    }
}

// class CodeStyleAnalyzer : DiagnosticAnalyzerCorrectnessAnalyzer
// {

// }

class CodeStyleDiagnosticReporter : IIncrementalGenerator
{
    static readonly IRule[] RULES = new[]
    {
        new FileLengthRule()
    };

    record struct Data(ImmutableArray<Diagnostic> Diagnostics);
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // var provider = context.SyntaxProvider
        //     .CreateSyntaxProvider<Data>(predicate, transform)
        //     .Where(static x => x != null);

        // context.RegisterSourceOutput(provider, execute);

        var files = context.AdditionalTextsProvider
            .Where(static file => Path.GetFileName(file.Path) == ".editorconfig")
            // .Select(static (text, token) => ) TODO
            .Collect();

        context.RegisterSourceOutput(files, execute);
    }

    private void execute(SourceProductionContext context, ImmutableArray<AdditionalText> array)
    {
        // context.
        throw new NotImplementedException();
    }
}
