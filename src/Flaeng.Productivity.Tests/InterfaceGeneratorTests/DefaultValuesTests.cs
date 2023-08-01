namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class DefaultValuesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public DefaultValuesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_parameter_with_default_value_default()
    {
        // Arrange
        string source = """
        using System.Threading;
        using System.Threading.Tasks;

        namespace TestNamespace;
        
        [Flaeng.GenerateInterface]
        public partial class Test
        {
            public Task MainAsync(CancellationToken token = default) { return Task.FromResult(new object()); }
        } 
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ITest
            {
                global::System.Threading.Tasks.Task MainAsync(
                    global::System.Threading.CancellationToken token = default
                );
            }
            public partial class Test : ITest
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
    public void can_handle_parameter_with_default_value_int()
    {
        // Arrange
        string source = """
        namespace TestNamespace;
        
        [Flaeng.GenerateInterface]
        public partial class Test
        {
            public void Main(int number = 1) {}
        } 
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ITest
            {
                void Main(
                    global::System.Int32 number = 1
                );
            }
            public partial class Test : ITest
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
    public void can_handle_parameter_with_default_value_string()
    {
        // Arrange
        string source = """
        namespace TestNamespace;
        
        [Flaeng.GenerateInterface]
        public partial class Test
        {
            public void Main(string str = "123") { }
        } 
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ITest
            {
                void Main(
                    global::System.String str = "123"
                );
            }
            public partial class Test : ITest
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
    public void can_handle_property_with_default_value_string()
    {
        // Arrange
        string source = """
        namespace TestNamespace;
        
        [Flaeng.GenerateInterface]
        public partial class Test
        {
            public string Main { get; set; } = "123";
        } 
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ITest
            {
                global::System.String Main { get; set; }
            }
            public partial class Test : ITest
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_handle_static_property_with_default_value_string()
    {
        // Arrange
        string source = """
        namespace TestNamespace;
        
        [Flaeng.GenerateInterface]
        public partial class Test
        {
            public static string Main { get; set; } = "123";
        } 
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ITest
            {
                static global::System.String Main { get; set; } = "123";
            }
            public partial class Test : ITest
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
