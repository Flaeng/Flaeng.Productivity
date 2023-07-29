namespace Flaeng.Productivity.Tests.StartupGeneratorTests;

public class NamespaceTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public NamespaceTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact]
    public void can_handle_filescoped_namespaces()
    {
        // Given
        string source = $$"""
        namespace TestNamespace.Providers;

        public interface IBlah { }

        [Flaeng.RegisterService]
        public class Blah : IBlah { }
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
                services.AddScoped<global::TestNamespace.Providers.IBlah, global::TestNamespace.Providers.Blah>();
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
    public void can_handle_no_namespace()
    {
        // Given
        string source = $$"""
        public interface IBlah { }

        [Flaeng.RegisterService]
        public class Blah : IBlah { }
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
                services.AddScoped<global::IBlah, global::Blah>();
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
    public void can_handle_normal_namespace()
    {
        // Given
        string source = $$"""
        namespace TestNamespace.Providers
        {
            public interface IBlah { }
            
            [Flaeng.RegisterService]
            public class Blah : IBlah { }
        }
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
                services.AddScoped<global::TestNamespace.Providers.IBlah, global::TestNamespace.Providers.Blah>();
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
