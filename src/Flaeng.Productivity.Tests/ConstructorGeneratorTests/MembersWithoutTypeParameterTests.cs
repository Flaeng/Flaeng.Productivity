namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class MembersWithoutTypeParameterTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MembersWithoutTypeParameterTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Theory]
    // readonly field
    [InlineData("private readonly IList _logger;")]
    [InlineData("private readonly IList _logger = new System.Collections.Generic.List<object>();")]
    [InlineData("protected readonly IList _logger;")]
    [InlineData("internal readonly IList _logger;")]
    [InlineData("public readonly IList _logger;")]
    [InlineData("readonly IList _logger;")]
    // // field
    [InlineData("private IList _logger;")]
    [InlineData("private IList _logger = new System.Collections.Generic.List<object>();")]
    [InlineData("protected IList _logger;")]
    [InlineData("internal IList _logger;")]
    [InlineData("public IList _logger;")]
    [InlineData("IList _logger;")]
    // // properties with getter
    [InlineData("private IList _logger { get; }")]
    [InlineData("private IList _logger { get; } = new System.Collections.Generic.List<object>();")]
    [InlineData("protected IList _logger { get; }")]
    [InlineData("internal IList _logger { get; }")]
    [InlineData("public IList _logger { get; }")]
    [InlineData("IList _logger { get; }")]
    // // properties with private setter
    [InlineData("protected IList _logger { get; private set; }")]
    [InlineData("internal IList _logger { get; private set; }")]
    [InlineData("public IList _logger { get; private set; }")]
    // // properties with init setter
    [InlineData("protected IList _logger { get; init; }")]
    [InlineData("internal IList _logger { get; init; }")]
    [InlineData("public IList _logger { get; init; }")]
    // properties with other getter/setter
    [InlineData("public IList _logger { internal get; set; }")]
    [InlineData("public IList _logger { protected get; set; }")]
    [InlineData("public IList _logger { private get; set; }")]
    public void can_parse_members_with_no_generic_parameter(string memberText)
    {
        // Arrange
        string source = $$"""
        using System.Collections;

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
            new SourceFile("Dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    IList logger
                )
                {
                    this._logger = logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(Path.Combine(Constants.ConstructorGeneratorGeneratedContentPathPrefix, "TestNamespace.Dummy.g.cs"), dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
    }

    [Theory]
    // readonly field
    [InlineData("private readonly static IList _logger;")]
    [InlineData("private readonly static IList _logger = new List<object>();")]
    [InlineData("protected readonly static IList _logger;")]
    [InlineData("internal readonly static IList _logger;")]
    [InlineData("public readonly static IList _logger;")]
    [InlineData("readonly static IList _logger;")]
    // field
    [InlineData("private static IList _logger;")]
    [InlineData("private static IList _logger = new List<object>();")]
    [InlineData("protected static IList _logger;")]
    [InlineData("internal static IList _logger;")]
    [InlineData("public static IList _logger;")]
    [InlineData("static IList _logger;")]
    // properties
    [InlineData("private static IList _logger { get; }")]
    [InlineData("private static IList _logger { get; } = new List<object>();")]
    [InlineData("protected static IList _logger { get; }")]
    [InlineData("internal static IList _logger { get; }")]
    [InlineData("public static IList _logger { get; }")]
    [InlineData("static IList _logger { get; }")]
    public void wont_parse_members_with_invalid_modifiers(string memberText)
    {
        // Arrange
        string source = $$"""
        using System.Collections;
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Flaeng.Inject] IList list;
                [Flaeng.Inject] {{memberText}}
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections;
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    IList list
                )
                {
                    this.list = list;
                }
            }
        }

        """;
        var error = output.Diagnostic.FirstOrDefault(x => x.Id == Rules.ConstructorGenerator_MemberIsStatic.Id);
        Assert.NotNull(error);
        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Equal("Design", error.Descriptor.Category);
        Assert.Equal("Members with Inject attribute cannot be static", error.Descriptor.Title);
        Assert.Contains("Member _logger cannot be static for the source generator to set it in the constructor", error.ToString());

        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(Path.Combine(Constants.ConstructorGeneratorGeneratedContentPathPrefix, "TestNamespace.Dummy.g.cs"), dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
    }

    [Fact(Timeout = 1000)]
    public void can_parse_members_with_custom_type()
    {
        // Arrange
        string serviceSource = """
        namespace TestNamespace.Services
        {
            public class TestService
            {
            }
        }
        """;
        string controllerSource = """
        using TestNamespace.Services;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject] 
                TestService _service;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Controller.cs", controllerSource),
            new SourceFile("Service.cs", serviceSource)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using TestNamespace.Services;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    TestService service
                )
                {
                    this._service = service;
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
    public void can_parse_members_with_multiple_attributes()
    {
        // Arrange
        string serviceSource = """
        namespace TestNamespace.Services
        {
            public class TestService
            {
            }
        }
        """;
        string controllerSource = """
        using TestNamespace.Services;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject] 
                [System.ComponentModel.DisplayName]
                TestService _service { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Controller.cs", controllerSource),
            new SourceFile("Service.cs", serviceSource)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using TestNamespace.Services;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    TestService service
                )
                {
                    this._service = service;
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
    public void can_parse_members_with_multiple_attributes_alternative()
    {
        // Arrange
        string serviceSource = """
        namespace TestNamespace.Services
        {
            public class TestService
            {
            }
        }
        """;
        string controllerSource = """
        using TestNamespace.Services;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject, System.ComponentModel.DisplayName]
                TestService _service { get; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Controller.cs", controllerSource),
            new SourceFile("Service.cs", serviceSource)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using TestNamespace.Services;

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    TestService service
                )
                {
                    this._service = service;
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
