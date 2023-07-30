namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class StaticMembersTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public StaticMembersTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_single_static_property()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public static string Simple { get; set; } = "123";
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                static global::System.String Simple { get; set; } = "123";
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_single_static_fields()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public static string Simple = "";
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                static global::System.String Simple = "";
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_single_static_method()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public static string GetSimple() => "123";
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                static abstract global::System.String GetSimple();
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

}
