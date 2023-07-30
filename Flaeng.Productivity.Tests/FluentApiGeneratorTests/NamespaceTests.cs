namespace Flaeng.Productivity.Tests.FluentApiGeneratorTests;

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

        [Flaeng.MakeFluent]
        public class Blah 
        { 
            public string Name;
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<FluentApiGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Then
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            public static partial class BlahExtensions
            {
                {{Constants.GeneratedCodeAttribute}}
                public static Blah Name(
                    this Blah _this,
                    global::System.String Name
                )
                {
                    _this.Name = Name;
                    return _this;
                }
            }
        }
        
        """;
        var files = output.GeneratedFiles.ExcludeTriggerAttribute();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(Constants.FluentApiGeneratorGeneratedContentPathPrefix + "TestNamespace.Providers.Blah.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact]
    public void can_handle_no_namespace()
    {
        // Given
        string source = $$"""
        [Flaeng.MakeFluent]
        public class Blah 
        { 
            public string Name;
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<FluentApiGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Then
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        public static partial class BlahExtensions
        {
            {{Constants.GeneratedCodeAttribute}}
            public static Blah Name(
                this Blah _this,
                global::System.String Name
            )
            {
                _this.Name = Name;
                return _this;
            }
        }
        
        """;
        var files = output.GeneratedFiles.ExcludeTriggerAttribute();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(Constants.FluentApiGeneratorGeneratedContentPathPrefix + "Blah.g.cs", file.Filename);
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
            [Flaeng.MakeFluent]
            public class Blah 
            { 
                public string Name;
            }
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<FluentApiGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Then
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            public static partial class BlahExtensions
            {
                {{Constants.GeneratedCodeAttribute}}
                public static Blah Name(
                    this Blah _this,
                    global::System.String Name
                )
                {
                    _this.Name = Name;
                    return _this;
                }
            }
        }
        
        """;
        var files = output.GeneratedFiles.ExcludeTriggerAttribute();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(Constants.FluentApiGeneratorGeneratedContentPathPrefix + "TestNamespace.Providers.Blah.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }

}
