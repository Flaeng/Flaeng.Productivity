namespace Flaeng.Productivity.Tests.StartupGeneratorTests;

public class MultipleServicesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleServicesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact]
    public void Can_handle_multiple_services()
    {
        // Given
        string source1 = """
        namespace Test
        {
            interface IDummy { }
            [Flaeng.RegisterService]
            class Dummy : IDummy { }
        }
        
        """;
        string source2 = """
        namespace Test;
        interface IDumdum { }
        [Flaeng.RegisterService]
        class Dumdum : IDumdum { }
        """;

        // When
        var output = compiler.GetGeneratedOutput<StartupGenerator>(
            new SourceFile("source1.cs", source1),
            new SourceFile("source2.cs", source2)
        );

        // Then
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
                services.AddScoped<global::Test.IDummy, global::Test.Dummy>();
                services.AddScoped<global::Test.IDumdum, global::Test.Dumdum>();
                return services;
            }
        }
        
        """;
        var file = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(file);
        Assert.Equal(Constants.StartupGeneratorGeneratedContentPathPrefix + "StartupExtensions.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }
}
