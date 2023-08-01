namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

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
        using System.Collections;
        using Flaeng;

        namespace TestNamespace;

        public partial class Dummy
        {
            [Inject] public IList _logger { get; }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    IList logger
                )
                {
                    this._logger = logger;
                }
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_handle_no_namespaces()
    {
        // Arrange
        string source = """
        using System.Collections;

        public partial class Dummy
        {
            [Flaeng.Inject] public IList _logger { get; }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections;

        public partial class Dummy
        {
            {{Constants.GeneratedCodeAttribute}}
            public Dummy(
                IList logger
            )
            {
                this._logger = logger;
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

}
