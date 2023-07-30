namespace Flaeng.Productivity.Tests.StartupGeneratorTests;

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
        namespace TestNamespace.Controllers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    [Flaeng.RegisterService] 
                    public class Dummy
                    {
                    }
                }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<StartupGenerator>(
            new SourceFile("Dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using Microsoft.Extensions.DependencyInjection;

        public static partial class StartupExtensions
        {
            {{Constants.GeneratedCodeAttribute}}
            public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection RegisterServices(
                this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services
            )
            {
                services.AddScoped<global::TestNamespace.Controllers.Wrapper1.Wrapper2.Dummy>();
                return services;
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(Constants.StartupGeneratorGeneratedContentPathPrefix + "StartupExtensions.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }


}
