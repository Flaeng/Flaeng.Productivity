using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flaeng.Productivity.Tests;

public record SourceFile
(
    string Filename,
    string Content
);
public record CompilationResult
(
    Compilation Output,
    ImmutableArray<SourceFile> GeneratedFiles,
    ImmutableArray<Diagnostic> Diagnostic
);

public class TestBase
{
    protected static CompilationResult GetGeneratedOutput<TSourceGenerator>(
        params SourceFile[] files
    )
        where TSourceGenerator : IIncrementalGenerator, new()
    {
        var syntaxTree = files.Select(x =>
                CSharpSyntaxTree.ParseText(x.Content, path: x.Filename)
            ).ToImmutableArray();

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference
                                .CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create("SourceGeneratorTests",
                      syntaxTree,
                      references,
                      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Source Generator to test
        var generator = new TSourceGenerator();

        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation,
                                            out var outputCompilation,
                                            out var diagnostics);

        // optional
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        if (errors.Any())
            throw new CompilationException(errors);

        return new CompilationResult(
            outputCompilation,
            outputCompilation.SyntaxTrees
                .Select(x => new SourceFile(x.FilePath, x.ToString()))
                .ToImmutableArray(),
            diagnostics
        );
    }


    protected ImmutableDictionary<string, string> Verify<TGenerator>(string source)
        where TGenerator : IIncrementalGenerator, new()
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        IEnumerable<PortableExecutableReference> references = new[]
        {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            };

        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);


        // Create an instance of our EnumGenerator incremental source generator
        var generator = new TGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation)
            .RunGeneratorsAndUpdateCompilation(
                compilation,
                out var output,
                out var diagnostics,
                CancellationToken.None);

        return new Dictionary<string, string>()
            .ToImmutableDictionary();
    }

    [Serializable]
    private class CompilationException : Exception
    {
        public IEnumerable<Diagnostic> Errors { get; }

        public CompilationException(IEnumerable<Diagnostic> errors)
        {
            Errors = errors;
        }
    }
}