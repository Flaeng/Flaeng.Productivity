namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class MembersWithTypeParametersTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MembersWithTypeParametersTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Theory]
    // readonly field
    [InlineData("private readonly List<string> _logger;")]
    [InlineData("protected readonly List<string> _logger;")]
    [InlineData("internal readonly List<string> _logger;")]
    [InlineData("public readonly List<string> _logger;")]
    [InlineData("readonly List<string> _logger;")]
    // field
    [InlineData("private List<string> _logger;")]
    [InlineData("protected List<string> _logger;")]
    [InlineData("internal List<string> _logger;")]
    [InlineData("public List<string> _logger;")]
    [InlineData("List<string> _logger;")]
    // properties
    [InlineData("private List<string> _logger { get; }")]
    [InlineData("protected List<string> _logger { get; }")]
    [InlineData("internal List<string> _logger { get; }")]
    [InlineData("public List<string> _logger { get; }")]
    [InlineData("List<string> _logger { get; }")]
    public void can_parse_members_with_one_generic_parameter(string memberText)
    {
        // Arrange
        string source = $$"""
        using System.Collections.Generic;
                
        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Flaeng.Inject] {{memberText}}
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace
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

        """;
        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_parse_members_with_one_qualified_generic_parameter()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject] 
                List<System.String> _list;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    List<System.String> list
                )
                {
                    this._list = list;
                }
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_parse_members_with_many_type_parameters()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject] 
                List<Dictionary<string, List<int>>> _list;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    List<Dictionary<string, List<int>>> list
                )
                {
                    this._list = list;
                }
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Theory]
    // readonly field
    [InlineData("private readonly IDictionary<string, object> _logger;")]
    [InlineData("protected readonly IDictionary<string, object> _logger;")]
    [InlineData("internal readonly IDictionary<string, object> _logger;")]
    [InlineData("public readonly IDictionary<string, object> _logger;")]
    [InlineData("readonly IDictionary<string, object> _logger;")]
    // field
    [InlineData("private IDictionary<string, object> _logger;")]
    [InlineData("protected IDictionary<string, object> _logger;")]
    [InlineData("internal IDictionary<string, object> _logger;")]
    [InlineData("public IDictionary<string, object> _logger;")]
    [InlineData("IDictionary<string, object> _logger;")]
    // properties
    [InlineData("private IDictionary<string, object> _logger { get; }")]
    [InlineData("protected IDictionary<string, object> _logger { get; }")]
    [InlineData("internal IDictionary<string, object> _logger { get; }")]
    [InlineData("public IDictionary<string, object> _logger { get; }")]
    [InlineData("IDictionary<string, object> _logger { get; }")]
    public void can_parse_members_with_multiple_generic_parameter(string memberText)
    {
        // Arrange
        string source = $$"""
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Flaeng.Inject] {{memberText}}
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    IDictionary<string, object> logger
                )
                {
                    this._logger = logger;
                }
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated?.Content);
        Assert.Empty(output.Diagnostic);
    }

}
