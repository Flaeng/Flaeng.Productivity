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
    protected static CompilationResult GetGeneratedOutput(
        IIncrementalGenerator[] generators,
        SourceFile[] sourceFiles
    )
    {
        var syntaxTree = sourceFiles.Select(x =>
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
                      //   new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                      new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        CSharpGeneratorDriver.Create(generators)
            .RunGeneratorsAndUpdateCompilation(compilation,
                                            out var outputCompilation,
                                            out var diagnostics);

        // optional
        // var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        // if (errors.Any())
        //     throw new CompilationException(errors);

        return new CompilationResult(
            outputCompilation,
            outputCompilation.SyntaxTrees
                .Select(x => new SourceFile(Path.GetFileName(x.FilePath), x.ToString()))
                .ToImmutableArray(),
            diagnostics
        );
    }

    protected static CompilationResult GetGeneratedOutput<TSourceGenerator>(
        params SourceFile[] files
    )
        where TSourceGenerator : IIncrementalGenerator, new()
    {
        return GetGeneratedOutput(
            new IIncrementalGenerator[] { new TSourceGenerator() },
            files
        );
    }

}