namespace Flaeng.Productivity.Tests.StartupGeneratorTests;

public class AttributeArgumentTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public AttributeArgumentTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Theory]
    [InlineData("[Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Transient)]")]
    [InlineData("[global::Flaeng.RegisterService(ServiceType = global::Flaeng.ServiceType.Transient)]")]
    [InlineData("""
    using Flaeng;
    [RegisterService(ServiceType = ServiceType.Transient)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterService(ServiceType = Test.ServiceType.Transient)]
    """)]
    public void Can_read_transient(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah { }
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
                services.AddTransient<global::Blah>();
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

    [Theory]
    [InlineData("[Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Transient)]")]
    [InlineData("[global::Flaeng.RegisterService(ServiceType = global::Flaeng.ServiceType.Transient)]")]
    [InlineData("""
    using Flaeng;
    [RegisterService(ServiceType = ServiceType.Transient)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterService(ServiceType = Test.ServiceType.Transient)]
    """)]
    public void Can_read_transient_with_interface(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah : IBlah { }
        public interface IBlah { }
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
                services.AddTransient<global::IBlah, global::Blah>();
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

    [Theory]
    [InlineData("[Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Scoped)]")]
    [InlineData("[global::Flaeng.RegisterService(ServiceType = global::Flaeng.ServiceType.Scoped)]")]
    [InlineData("""
    using Flaeng;
    [RegisterService(ServiceType = ServiceType.Scoped)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterService(ServiceType = Test.ServiceType.Scoped)]
    """)]
    public void Can_read_scoped(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah { }
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
                services.AddScoped<global::Blah>();
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

    [Theory]
    [InlineData("[Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Scoped)]")]
    [InlineData("[Flaeng.RegisterServiceAttribute(ServiceType = Flaeng.ServiceType.Scoped)]")]
    [InlineData("[global::Flaeng.RegisterService(ServiceType = global::Flaeng.ServiceType.Scoped)]")]
    [InlineData("[global::Flaeng.RegisterServiceAttribute(ServiceType = global::Flaeng.ServiceType.Scoped)]")]
    [InlineData("""
    using Flaeng;
    [RegisterService(ServiceType = ServiceType.Scoped)]
    """)]
    [InlineData("""
    using Flaeng;
    [RegisterServiceAttribute(ServiceType = ServiceType.Scoped)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterService(ServiceType = Test.ServiceType.Scoped)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterServiceAttribute(ServiceType = Test.ServiceType.Scoped)]
    """)]
    public void Can_read_scoped_with_interface(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah : IBlah { }
        public interface IBlah { }
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

    [Theory]
    [InlineData("[Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Singleton)]")]
    [InlineData("[Flaeng.RegisterServiceAttribute(ServiceType = Flaeng.ServiceType.Singleton)]")]
    [InlineData("[global::Flaeng.RegisterService(ServiceType = global::Flaeng.ServiceType.Singleton)]")]
    [InlineData("[global::Flaeng.RegisterServiceAttribute(ServiceType = global::Flaeng.ServiceType.Singleton)]")]
    [InlineData("""
    using Flaeng;
    [RegisterService(ServiceType = ServiceType.Singleton)]
    """)]
    [InlineData("""
    using Flaeng;
    [RegisterServiceAttribute(ServiceType = ServiceType.Singleton)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterService(ServiceType = Test.ServiceType.Singleton)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterServiceAttribute(ServiceType = Test.ServiceType.Singleton)]
    """)]
    public void Can_read_singleton(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah { }
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
                services.AddSingleton<global::Blah>();
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

    [Theory]
    [InlineData("[Flaeng.RegisterService(ServiceType = Flaeng.ServiceType.Singleton)]")]
    [InlineData("[Flaeng.RegisterServiceAttribute(ServiceType = Flaeng.ServiceType.Singleton)]")]
    [InlineData("[global::Flaeng.RegisterService(ServiceType = global::Flaeng.ServiceType.Singleton)]")]
    [InlineData("[global::Flaeng.RegisterServiceAttribute(ServiceType = global::Flaeng.ServiceType.Singleton)]")]
    [InlineData("""
    using Flaeng;
    [RegisterService(ServiceType = ServiceType.Singleton)]
    """)]
    [InlineData("""
    using Flaeng;
    [RegisterServiceAttribute(ServiceType = ServiceType.Singleton)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterService(ServiceType = Test.ServiceType.Singleton)]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.RegisterServiceAttribute(ServiceType = Test.ServiceType.Singleton)]
    """)]
    public void Can_read_singleton_with_interface(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah : IBlah { }
        public interface IBlah { }
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
                services.AddSingleton<global::IBlah, global::Blah>();
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
