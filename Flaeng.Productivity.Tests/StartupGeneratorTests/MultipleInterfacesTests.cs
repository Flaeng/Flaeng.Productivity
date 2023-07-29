namespace Flaeng.Productivity.Tests.StartupGeneratorTests;

public class MultipleInterfacesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleInterfacesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact]
    public void can_handle_multiple_interfaces_in_same_file()
    {
        // Given
        string source = $$"""
        namespace TestNamespace.Providers;

        public interface IBlah1 { }
        public interface IBlah2 { }
        [Flaeng.RegisterService]
        public class Blah : IBlah1, IBlah2 { }
        """;

        // When
        var output = compiler.GetGeneratedOutput<StartupGenerator>(
            new SourceFile("dummy.cs", source)
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
                services.AddScoped<global::TestNamespace.Providers.IBlah1, global::TestNamespace.Providers.Blah>();
                services.AddScoped<global::TestNamespace.Providers.IBlah2, global::TestNamespace.Providers.Blah>();
                return services;
            }
        }
        
        """;
        var files = output.GeneratedFiles.ExcludeTriggerAttribute();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(Constants.StartupGeneratorGeneratedContentPathPrefix + "StartupExtensions.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }
    
    [Fact]
    public void can_handle_multiple_interfaces_in_different_files()
    {
        // Given
        string source1 = $$"""
        namespace TestNamespace.Providers;

        public interface IBlah1 { }
        """;
        string source2 = $$"""
        namespace TestNamespace.Providers;

        public interface IBlah2 { }
        """;
        string source3 = $$"""
        namespace TestNamespace.Providers;

        [Flaeng.RegisterService]
        public class Blah : IBlah1, IBlah2 { }
        """;

        // When
        var output = compiler.GetGeneratedOutput<StartupGenerator>(
            new SourceFile("source1.cs", source1),
            new SourceFile("source2.cs", source2),
            new SourceFile("source3.cs", source3)
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
                services.AddScoped<global::TestNamespace.Providers.IBlah1, global::TestNamespace.Providers.Blah>();
                services.AddScoped<global::TestNamespace.Providers.IBlah2, global::TestNamespace.Providers.Blah>();
                return services;
            }
        }
        
        """;
        var files = output.GeneratedFiles.ExcludeTriggerAttribute();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(Constants.StartupGeneratorGeneratedContentPathPrefix + "StartupExtensions.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }
    
}
