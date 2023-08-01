namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class MultipleClassesInSameFileTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleClassesInSameFileTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_multiple_classes_in_one_file()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject] 
                global::System.Collections.Generic.List<System.String> _logger;
            }
            public partial class DumDum
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
        string expected_output1 = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            public partial class Dummy
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
        string expected_output2 = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum
            {
                {{Constants.GeneratedCodeAttribute}}
                public DumDum(
                    global::System.Collections.Generic.List<System.String> logger
                )
                {
                    this._logger = logger;
                }
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("Dummy.g.cs"));
        Assert.Equal(expected_output1, dummyGenerated?.Content);

        var dumdumGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("DumDum.g.cs"));
        Assert.Equal(expected_output2, dumdumGenerated?.Content);

        Assert.Empty(output.Diagnostic);
    }

}
