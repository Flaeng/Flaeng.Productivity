using System.Diagnostics;

namespace Flaeng.Productivity.Tests;

public record SourceFile
(
    string Filename,
    string Content
)
{
    public static SourceFile Parse(SyntaxTree x)
    {
        var filename = x.FilePath;
        return new SourceFile(filename, x.ToString());
    }
}

public record CompilationResult
(
    Compilation Output,
    ImmutableArray<SourceFile> CompilatedFiles,
    ImmutableArray<SourceFile> GeneratedFiles,
    ImmutableArray<Diagnostic> Diagnostic,
    ImmutableArray<Diagnostic> Errors,
    ImmutableArray<Diagnostic> Warnings
);

public class CSharpCompiler
{
    public CompilationResult GetGeneratedOutput<T>(
        params SourceFile[] sourceFiles
        )
        where T : IIncrementalGenerator, new()
    {
#nullable disable
        var stacktrace = new StackTrace();
        var frame = stacktrace.GetFrame(1);
        _ = frame.GetFileName();
        var method = frame.GetMethod();
        string callerMethodName = method.Name;
#nullable enable
        return getGeneratedOutput(
            new IIncrementalGenerator[] { new T() },
            sourceFiles,
            callerMethodName
            );
    }

    public CompilationResult GetGeneratedOutput<T1, T2>(
        params SourceFile[] sourceFiles
        )
        where T1 : IIncrementalGenerator, new()
        where T2 : IIncrementalGenerator, new()
    {
#nullable disable
        var stacktrace = new StackTrace();
        var frame = stacktrace.GetFrame(1);
        _ = frame.GetFileName();
        var method = frame.GetMethod();
        _ = method.Name;
#nullable enable
        return GetGeneratedOutput(
            new IIncrementalGenerator[] { new T1(), new T2() },
            sourceFiles
            );
    }

    public CompilationResult GetGeneratedOutput(
        IIncrementalGenerator[] generators,
        params SourceFile[] sourceFiles
        )
    {
#nullable disable
        var stacktrace = new StackTrace();
        var frame = stacktrace.GetFrame(1);
        _ = frame.GetFileName();
        var method = frame.GetMethod();
        string callerMethodName = method.Name;
#nullable enable
        return getGeneratedOutput(generators, sourceFiles, callerMethodName);
    }

    private CompilationResult getGeneratedOutput(
        IIncrementalGenerator[] generators,
        SourceFile[] sourceFiles,
        string caller
        )
    {

        var syntaxTreeArr = sourceFiles.Select(x =>
                CSharpSyntaxTree.ParseText(
                    text: x.Content,
                    path: x.Filename
                    )
            )
            .ToImmutableArray();

        var referenceArr = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>()
            .ToImmutableArray();

        var driver1compilation = CSharpCompilation.Create(
                    assemblyName: "SourceGeneratorTests",
                    syntaxTreeArr,
                    referenceArr,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver1 = CSharpGeneratorDriver.Create(generators)
            .RunGeneratorsAndUpdateCompilation(
                driver1compilation,
                out var driver1output,
                out var driver1diragnostics
                );

        var driver1compilationDiagnostics = driver1compilation.GetDiagnostics();
        var driv1outputDiagnostics = driver1output.GetDiagnostics();

        var driver1runResult = driver1.GetRunResult();
        var driver1firstRunResult = driver1runResult.Results.FirstOrDefault();
        var driver1firstRunDiagnostics = driver1firstRunResult.Diagnostics;

        var driver2 = CSharpGeneratorDriver.Create(generators)
            .RunGeneratorsAndUpdateCompilation(
                driver1output,
                out _,
                out var driver2diragnostics
                );

        var driver2runResult = driver2.GetRunResult();
        var driver2firstRunResult = driver2runResult.Results.FirstOrDefault();
        var driver2firstRunDiagnostics = driver2firstRunResult.Diagnostics;

        var outputCompilationFiles = driver1output.SyntaxTrees
                .Select(x => SourceFile.Parse(x))
                .ToImmutableArray();

        List<Diagnostic> diagnostics = new();
        if (driver2firstRunDiagnostics != default)
            diagnostics.AddRange(driver2firstRunDiagnostics);
        if (driver2diragnostics != default)
            diagnostics.AddRange(driver2diragnostics);
        if (driv1outputDiagnostics != default)
            diagnostics.AddRange(driv1outputDiagnostics);
        if (driver1firstRunDiagnostics != default)
            diagnostics.AddRange(driver1firstRunDiagnostics);

        // if (Debugger.IsAttached)
        // {
        //     var outputDirectory = Path.Combine("CompilationOutput", caller);
        //     WriteOutputToFileSystem(outputDirectory, outputCompilationFiles);
        // }

        // var diagnostics = driver2firstRunDiagnostics.Concat(driver2diragnostics).Concat(driv1outputDiagnostics).Concat(driver1firstRunDiagnostics).ToImmutableArray();

        return new CompilationResult(
            driver1output,
            outputCompilationFiles,
            outputCompilationFiles.Where(x => x.Filename.EndsWith(".g.cs")).ToImmutableArray(),
            diagnostics.ToImmutableArray(),
            diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).ToImmutableArray(),
            diagnostics.Where(x => x.Severity == DiagnosticSeverity.Warning).ToImmutableArray()
        );
    }

    private void WriteOutputToFileSystem(string outputDirectory, ImmutableArray<SourceFile> outputCompilationFiles)
    {
        var root = GetRootDirectory();
        var outputPath = Path.Combine(root, outputDirectory);
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath, true);

        if (outputCompilationFiles.Length == 0)
            return;

        foreach (var file in outputCompilationFiles)
        {
            var filepath = Path.Combine(outputPath, file.Filename);
            CreateFolder(filepath);
            File.WriteAllText(filepath, file.Content);
        }

        WriteCsproj(outputPath);
    }

    private static void WriteCsproj(string outputPath)
    {
        File.WriteAllText(Path.Combine(outputPath, "project.csproj"), """
        <Project Sdk="Microsoft.NET.Sdk">

            <PropertyGroup>
                <TargetFramework>net7.0</TargetFramework>
                <ImplicitUsings>disable</ImplicitUsings>
                <Nullable>enable</Nullable>
            </PropertyGroup>

        </Project>
        """);
    }

    private void CreateFolder(string filepath)
    {
        var di = new FileInfo(filepath).Directory;
        if (di is null)
            throw new IOException("Failed to find directory of filepath");

        if (di.Parent is null)
            throw new IOException("Failed to find directory parent of filepath");

        if (di.Parent.Exists == false)
            CreateFolder(di.Parent.FullName);

        Directory.CreateDirectory(di.FullName);
    }

    private static string GetRootDirectory()
    {
        var dir = Directory.GetCurrentDirectory();
        var di = new DirectoryInfo(dir);
        while (di is not null && di.Name != "Flaeng.Productivity")
            di = di.Parent;

        return di is null ? throw new IOException("Failed to find root directory") : di.FullName;
    }

}
