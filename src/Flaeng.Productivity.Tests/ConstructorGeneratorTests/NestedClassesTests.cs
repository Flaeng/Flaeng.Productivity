namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class NestedClassesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public NestedClassesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_nested_classes()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    public partial class Dummy
                    {
                        [Flaeng.Inject] 
                        List<string> _logger;
                    }
                }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    public partial class Dummy
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
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(Constants.ConstructorGeneratorGeneratedContentPathPrefix + "TestNamespace.Controllers.Wrapper1.Wrapper2.Dummy.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }


}
