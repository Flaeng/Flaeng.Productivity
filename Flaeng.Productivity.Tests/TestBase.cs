using System.Collections.Immutable;
using System.Runtime.CompilerServices;

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
        SourceFile[] sourceFiles,
        [CallerMemberName] string? callerMemberName = null
    )
    {
        if (callerMemberName == ".ctor")
        {
            throw new ArgumentException(nameof(callerMemberName));
        }

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
                      new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        CSharpGeneratorDriver.Create(generators)
            .RunGeneratorsAndUpdateCompilation(compilation,
                                            out var outputCompilation,
                                            out var diagnostics);

        var outputCompilationFiles = outputCompilation.SyntaxTrees
                .Select(x => new SourceFile(Path.GetFileName(x.FilePath), x.ToString()))
                .ToImmutableArray();

        // Future feature: Autogenerate examples of input and generated output
        // if (callerMemberName != null)
        // {
        //     var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        //     dir = dir.CreateSubdirectory("output");

        //     var subdir = dir.GetDirectories(callerMemberName).SingleOrDefault();
        //     if (subdir != null)
        //         subdir.Delete(recursive: true);
        //     subdir = dir.CreateSubdirectory(callerMemberName);

        //     foreach (var file in outputCompilationFiles)
        //     {
        //         var filepath = Path.Combine(subdir.FullName, file.Filename);
        //         File.WriteAllLines(filepath, file.Content.Split(Environment.NewLine));
        //     }
        // }

        return new CompilationResult(
            outputCompilation,
            outputCompilationFiles,
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
