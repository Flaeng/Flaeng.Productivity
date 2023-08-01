namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class VisibilityTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public VisibilityTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_missing_visiblity()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            partial class Dummy
            {
                [Flaeng.Inject] 
                List<string> _logger;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            internal partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    List<string> logger
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
    public void will_inherit_visiblity()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Controllers
        {
            internal partial class Dummy
            {
                [Flaeng.Inject] 
                global::System.Collections.Generic.List<System.String> _logger;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            internal partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    global::System.Collections.Generic.List<System.String> logger
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

}
