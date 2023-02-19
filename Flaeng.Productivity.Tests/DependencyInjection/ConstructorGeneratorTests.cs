using Flaeng.Productivity.DependencyInjection;

namespace Flaeng.Productivity.Tests.DependencyInjection;

public class ConstructorGeneratorTests : TestBase
{
    [Fact]
    public void Does_generate_attribute()
    {
        // Arrange

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>();

        // Assert
        Assert.Empty(output.Diagnostic);

        var source = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("InjectAttribute.g.cs"));
        Assert.Equal("""
        // <auto-generated/>
        #nullable enable

        namespace Flaeng.Productivity.DependencyInjection
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
            [global::System.AttributeUsageAttribute(
                global::System.AttributeTargets.Property | global::System.AttributeTargets.Field, 
                AllowMultiple = false, 
                Inherited = false)]
            internal sealed class InjectAttribute : global::System.Attribute
            {
                public InjectAttribute()
                { }
            }
        }
        """, source?.Content);
    }

    [Theory]
    // readonly field
    [InlineData("private readonly IList _logger;")]
    [InlineData("protected readonly IList _logger;")]
    [InlineData("internal readonly IList _logger;")]
    [InlineData("public readonly IList _logger;")]
    [InlineData("readonly IList _logger;")]
    // field
    [InlineData("private IList _logger;")]
    [InlineData("protected IList _logger;")]
    [InlineData("internal IList _logger;")]
    [InlineData("public IList _logger;")]
    [InlineData("IList _logger;")]
    // properties
    [InlineData("private IList _logger { get; }")]
    [InlineData("protected IList _logger { get; }")]
    [InlineData("internal IList _logger { get; }")]
    [InlineData("public IList _logger { get; }")]
    [InlineData("IList _logger { get; }")]
    public void can_parse_members_with_no_generic_parameter(string memberText)
    {
        // Arrange
        string source = @$"using System.Collections;

namespace TestNamespace
{{
    public partial class Dummy
    {{
        [Flaeng.Productivity.DependencyInjection.Inject] {memberText}
    }}
}}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        using System.Collections;

        #nullable enable

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    IList _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("Dummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void wont_generate_source_when_no_attribute_is_provided()
    {
        // Arrange
        string source = @$"using System.Collections;

namespace TestNamespace
{{
    public partial class Dummy
    {{
        public IList _logger {{ get; }}
    }}
}}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.Diagnostic);
        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("Dummy.g.cs"));
        Assert.Null(dummyGenerated);
    }

    [Fact]
    public void can_handle_filescoped_namespaces()
    {
        // Arrange
        string source = @$"using System.Collections;
using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace;

public partial class Dummy
{{
    [Inject] public IList _logger {{ get; }}
}}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        using System.Collections;
        using Flaeng.Productivity.DependencyInjection;

        #nullable enable

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    IList _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Dummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_handle_no_namespaces()
    {
        // Arrange
        string source = @$"using System.Collections;
using Flaeng.Productivity.DependencyInjection;

public partial class Dummy
{{
    [Inject] public IList _logger {{ get; }}
}}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        using System.Collections;
        using Flaeng.Productivity.DependencyInjection;

        #nullable enable

        public partial class Dummy
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
            public Dummy(
                IList _logger
                )
            {
                this._logger = _logger;
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("Dummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
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
        string source = @$"using System.Collections.Generic;
        
namespace TestNamespace
{{
    public partial class Dummy
    {{
        [Flaeng.Productivity.DependencyInjection.Inject] {memberText}
    }}
}}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        using System.Collections.Generic;

        #nullable enable

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    List<string> _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_parse_members_with_one_qualified_generic_parameter()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Productivity.DependencyInjection.Inject] 
                global::Microsoft.Extensions.Logging.ILogger<System.String> _logger;
            }
        }
        """;

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        #nullable enable

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    global::Microsoft.Extensions.Logging.ILogger<System.String> _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void will_inherit_visiblity()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Controllers
        {
            internal partial class Dummy
            {
                [Flaeng.Productivity.DependencyInjection.Inject] 
                global::Microsoft.Extensions.Logging.ILogger<System.String> _logger;
            }
        }
        """;

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        #nullable enable

        namespace TestNamespace.Controllers
        {
            internal partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    global::Microsoft.Extensions.Logging.ILogger<System.String> _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_handle_missing_visiblity()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Controllers
        {
            partial class Dummy
            {
                [Flaeng.Productivity.DependencyInjection.Inject] 
                global::Microsoft.Extensions.Logging.ILogger<System.String> _logger;
            }
        }
        """;

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        #nullable enable

        namespace TestNamespace.Controllers
        {
            partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    global::Microsoft.Extensions.Logging.ILogger<System.String> _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_handle_nested_classes()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Controllers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    public partial class Dummy
                    {
                        [Flaeng.Productivity.DependencyInjection.Inject] 
                        global::Microsoft.Extensions.Logging.ILogger<System.String> _logger;
                    }
                }
            }
        }
        """;

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        #nullable enable

        namespace TestNamespace.Controllers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    public partial class Dummy
                    {
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                        public Dummy(
                            global::Microsoft.Extensions.Logging.ILogger<System.String> _logger
                            )
                        {
                            this._logger = _logger;
                        }
                    }
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
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
        string source = @$"using System;
using System.Collections.Generic;
using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace
{{
    public partial class Dummy
    {{
        [Inject] {memberText}
    }}
}}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        using System;
        using System.Collections.Generic;
        using Flaeng.Productivity.DependencyInjection;

        #nullable enable

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    IDictionary<string, object> _logger
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact(Skip = "Not implemented yet")]
    public void Can_handle_partial_classes_in_multiple_files()
    {
        // Arrange
        string source1 = """
        using System;
        using Namespace1;
        using System.Collections.Generic;
        using Flaeng.Productivity.DependencyInjection;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Inject] IDictionary<string, object> _logger1;
            }
        }
        """;
        string source2 = """
        using System;
        using Namespace2;
        using System.Collections.Generic;
        using Flaeng.Productivity.DependencyInjection;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Inject] IDictionary<string, object> _logger2;
            }
        }
        """;

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy1.cs", source1),
            new SourceFile("dummy2.cs", source2)
        );

        // Assert
        string expected_output = """
        // <auto-generated/>

        using System;
        using Namespace1;
        using Namespace2;
        using System.Collections.Generic;
        using Flaeng.Productivity.DependencyInjection;

        #nullable enable

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Flaeng.Productivity", "1.0.0.0")]
                public Dummy(
                    IDictionary<string, object> _logger1,
                    IDictionary<string, object> _logger2,
                    )
                {
                    this._logger = _logger;
                }
            }
        }

        """;
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("ummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

}
