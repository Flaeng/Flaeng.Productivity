namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class WontGenerateSourceTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public WontGenerateSourceTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void wont_generate_source_when_no_attribute_is_provided()
    {
        // Arrange
        string source = """
        using System.Collections;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                public IList _logger { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void Shows_diagnotics_if_class_is_not_partial()
    {
        // Arrange
        string source = """
        using System.Collections;

        namespace TestNamespace
        {
            public class Dummy
            {
                [Flaeng.Inject] public IList _logger { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());

        var error = output.Diagnostic.FirstOrDefault(x => x.Id == Rules.ConstructorGenerator_ClassIsNotPartial.Id);
        Assert.NotNull(error);
        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Equal("Constructor", error.Descriptor.Category);
        Assert.Equal("Classes with Inject attribute on members should be partial", error.Descriptor.Title);
        Assert.Contains("Class Dummy should be partial for the source generator to extend it", error.ToString());
    }

    [Fact(Timeout = 1000)]
    public void Shows_diagnotics_if_class_is_static()
    {
        // Arrange
        string source = """
        using System.Collections;

        namespace TestNamespace
        {
            public static partial class Dummy
            {
                [Flaeng.Inject] public IList _logger { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());

        var error = output.Diagnostic.FirstOrDefault(x => x.Id == Rules.ConstructorGenerator_ClassIsStatic.Id);
        Assert.NotNull(error);
        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Equal("Constructor", error.Descriptor.Category);
        Assert.Equal("Classes with Inject attribute on members cannot be static", error.Descriptor.Title);
        Assert.Contains("Class Dummy cannot be static for the source generator to extend it", error.ToString());
    }

}
