namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class TypeParameterTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public TypeParameterTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void will_handle_generic_parameter_with_nested_generic_types()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class CanDoStuff<T>
            {
                public Dictionary<string, IList<T>> DoStuff(KeyValuePair<string, T> data) 
                { 
                    return new Dictionary<string, IList<T>>(); 
                }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("CanDoStuff.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ICanDoStuff<T>
            {
                global::System.Collections.Generic.Dictionary<global::System.String, global::System.Collections.Generic.IList<T>> DoStuff(
                    global::System.Collections.Generic.KeyValuePair<global::System.String, T> data
                );
            }
            public partial class CanDoStuff<T> : ICanDoStuff<T>
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.EndsWith("TestNamespace.Providers.ICanDoStuff.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void will_handle_multiple_type_parameters()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class CanDoStuff<T1, T2>
            {
                public Dictionary<string, IList<T2>> DoStuff(KeyValuePair<string, T1> data) 
                { 
                    return new Dictionary<string, IList<T2>>(); 
                }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("CanDoStuff.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ICanDoStuff<T1, T2>
            {
                global::System.Collections.Generic.Dictionary<global::System.String, global::System.Collections.Generic.IList<T2>> DoStuff(
                    global::System.Collections.Generic.KeyValuePair<global::System.String, T1> data
                );
            }
            public partial class CanDoStuff<T1, T2> : ICanDoStuff<T1, T2>
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.EndsWith("TestNamespace.Providers.ICanDoStuff.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void will_handle_inherited_methods_with_generic_parameter()
    {
        // Arrange
        string source1 = """
        namespace TestNamespace.Providers
        {
            public abstract class CanLogout<T>
            {
                public void Logout(T data) { }
            }
        }
        """;
        string source2 = """
        namespace TestNamespace.Providers
        {
            public abstract class Auth<TUser, TLoginInfo> : CanLogout<TUser>
            {
                public TUser CurrentUser { get; }
                public TUser Login(TLoginInfo data) { return default; }
                public TUser Login(string username, string password) { return default; }
            }
        }
        """;
        string source3 = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy : Auth<User, Login>
            {
            }
            public class User { }
            public class Login { }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("canlogout.cs", source1),
            new SourceFile("auth.cs", source2),
            new SourceFile("dummy.cs", source3)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                global::TestNamespace.Providers.User CurrentUser { get; }
                global::TestNamespace.Providers.User Login(
                    global::TestNamespace.Providers.Login data
                );
                global::TestNamespace.Providers.User Login(
                    global::System.String username,
                    global::System.String password
                );
                void Logout(
                    global::TestNamespace.Providers.User data
                );
            }
            public partial class Dummy : IDummy
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.EndsWith("TestNamespace.Providers.IDummy.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void will_handle_generic_parameter_with_multiple_generic_types()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;

        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class CanDoStuff<T>
            {
                public Dictionary<string, T> DoStuff(KeyValuePair<string, T> data) 
                {
                    return new Dictionary<string, T>();
                }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("CanDoStuff.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ICanDoStuff<T>
            {
                global::System.Collections.Generic.Dictionary<global::System.String, T> DoStuff(
                    global::System.Collections.Generic.KeyValuePair<global::System.String, T> data
                );
            }
            public partial class CanDoStuff<T> : ICanDoStuff<T>
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.EndsWith("TestNamespace.Providers.ICanDoStuff.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

}
