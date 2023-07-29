namespace Flaeng.Productivity.Tests.StartupGeneratorTests;

public class SimpleTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public SimpleTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void Can_make_startup_extension_with_one_class()
    {
        // Given
        string source = """
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
    
    [Fact(Timeout = 1000)]
    public void Can_make_startup_extension_with_two_class()
    {
        // Given
        string source = """
        public interface IBlah1 { }
        [Flaeng.RegisterService]
        public class Blah1 : IBlah1 { }

        public interface IBlah2 { }
        [Flaeng.RegisterService]
        public class Blah2 : IBlah2 { }
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
                services.AddScoped<global::IBlah1, global::Blah1>();
                services.AddScoped<global::IBlah2, global::Blah2>();
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
    
    [Fact(Timeout = 1000)]
    public void Can_handle_partial_class_with_multiple_interfaces_in_same_file()
    {
        // Given
        string source = """
        public interface IBlah1 { }
        [Flaeng.RegisterService]
        public partial class Blah : IBlah1 { }

        public interface IBlah2 { }
        public partial class Blah : IBlah2 { }
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
                services.AddScoped<global::IBlah1, global::Blah>();
                services.AddScoped<global::IBlah2, global::Blah>();
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
    
    [Fact(Timeout = 1000)]
    public void Can_handle_partial_class_with_multiple_interfaces_in_different_files()
    {
        // Given
        string source1 = """
        public interface IBlah1 { }
        [Flaeng.RegisterService]
        public partial class Blah : IBlah1 { }
        """;

        string source2 = """
        public interface IBlah2 { }
        public partial class Blah : IBlah2 { }
        """;

        // When
        var output = compiler.GetGeneratedOutput<StartupGenerator>(
            new SourceFile("dummy1.cs", source1),
            new SourceFile("dummy2.cs", source2)
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
                services.AddScoped<global::IBlah1, global::Blah>();
                services.AddScoped<global::IBlah2, global::Blah>();
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

