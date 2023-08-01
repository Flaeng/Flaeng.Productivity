namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class NamespaceTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public NamespaceTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_filescoped_namespaces()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers;
        [Flaeng.GenerateInterface]
        public partial class Dummy
        { }
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
    public void can_make_interface_with_no_namespace()
    {
        // Arrange
        string source = """
        [Flaeng.GenerateInterface]
        public partial class Dummy
        { }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        {{Constants.GeneratedCodeAttribute}}
        public interface IDummy
        {
        }
        public partial class Dummy : IDummy
        {
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

}
