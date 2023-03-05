using Flaeng.Productivity.DependencyInjection;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity.Tests.UseCases;

public abstract class BaseUseCase : TestBase
{
    protected CompilationResult Result { get; }

    public BaseUseCase(string directoryName)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var sourceFiles = new DirectoryInfo(currentDirectory)
            .Parent!
            .Parent!
            .Parent!
            .GetFiles($"UseCases/{directoryName}/*.txt")
            .Select(x =>
            {
                using var reader = new StreamReader(x.OpenRead());
                return new SourceFile(x.Name, reader.ReadToEnd());
            })
            .ToArray();

        Result = GetGeneratedOutput(
            new IIncrementalGenerator[]
            {
                new ConstructorGenerator(),
                new InterfaceGenerator(),
            },
            sourceFiles,
            this.GetType().Name
        );
    }
}
