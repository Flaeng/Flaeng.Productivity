namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

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
        namespace TestNamespace.Providers
        {
            public partial class Dummy
            {
                public void Simple() { }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.Diagnostic);
        // Should only contain generated attribute
        Assert.Single(output.GeneratedFiles);
    }

    [Fact(Timeout = 1000)]
    public void wont_generate_source_when_struct_is_provided()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            public partial struct Dummy
            {
                public bool Simple;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.Diagnostic);
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_no_methods()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            { }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void wont_make_interface_with_single_protected_property()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                protected string Simple { get; set; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
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
            [Flaeng.GenerateInterface]
            public class Dummy
            {
                public IList _logger { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());
        
        var error = output.Diagnostic.FirstOrDefault(x => x.Id == Rules.InterfaceGenerator_ClassIsNotPartial.Id);
        Assert.NotNull(error);
        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Equal("Design", error.Descriptor.Category);
        Assert.Equal("Classes with GenerateInterface attribute should be partial", error.Descriptor.Title);
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
            [Flaeng.GenerateInterface]
            public static partial class Dummy
            {
                public static IList _logger { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());
        
        var error = output.Diagnostic.FirstOrDefault(x => x.Id == Rules.InterfaceGenerator_ClassIsStatic.Id);
        Assert.NotNull(error);
        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Equal("Design", error.Descriptor.Category);
        Assert.Equal("Classes with GenerateInterface attribute cannot be static", error.Descriptor.Title);
        Assert.Contains("Class Dummy cannot be static for the source generator to extend it", error.ToString());
    }

}
