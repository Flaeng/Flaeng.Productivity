namespace Flaeng.Productivity.Tests;

public class SourceBuilderTests
{

    [Fact]
    public void Can_make_empty_source()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable
                     """, result);
    }

    [Fact]
    public void Can_change_tab_size()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.TabLength = 2;
        sourceBuilder.StartNamespace("Test");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("TestClass"));
        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace Test
                     {
                       public class TestClass
                       {
                       }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_use_tabs()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.TabLength = 1;
        sourceBuilder.TabStyle = TabStyle.Tabs;
        sourceBuilder.StartNamespace("Test");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("TestClass"));
        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace Test
                     {
                     
                     """ +
                     '\t' + "public class TestClass" + Environment.NewLine +
                     '\t' + "{" + Environment.NewLine +
                     '\t' + "}" + Environment.NewLine +
                     """
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_disable_nullable()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.NullableEnable = false;
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable disable
                     """, result);
    }

    [Fact]
    public void Can_make_empty_namespace()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_class()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("Publicy"));
        classBuilder.StartMethod(new MethodOptions("string", "Parse")
        {
            Visibility = MemberVisibility.Private,
            Static = true
        });
        classBuilder.AddLineOfCode("return \"\";");
        classBuilder.EndMethod();
        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public class Publicy
                         {
                             private static string Parse()
                             {
                                 return "";
                             }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_static_class()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("Publicy")
        {
            Visibility = TypeVisibility.Public,
            Static = true,
            Interfaces = new[] { "BaseClass", "Interface1", "Interface2" }
        });
        classBuilder.StartMethod(new MethodOptions("string", "Parse")
        {
            Visibility = MemberVisibility.Private,
            Static = true
        });
        sourceBuilder.AddLineOfCode("return \"\";");
        classBuilder.EndMethod();
        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public static class Publicy : BaseClass, Interface1, Interface2
                         {
                             private static string Parse()
                             {
                                 return "";
                             }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_interface()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var interfaceBuilder = sourceBuilder.StartInterface(new InterfaceOptions("Interconnected")
        {
            Visibility = TypeVisibility.Internal,
            Partial = true,
            Interfaces = new[] { "BaseInterface" }
        });
        interfaceBuilder.AddMethodStub(new MethodOptions("void", "Invoke")
        {
            Parameters = new List<string>(new[] { "object obj", "string str" })
        });
        interfaceBuilder.EndInterface();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         internal partial interface Interconnected : BaseInterface
                         {
                             void Invoke(
                                 object obj,
                                 string str
                                 );
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_struct()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var structBuilder = sourceBuilder.StartStruct(new StructOptions("DataContainer")
        {
            Visibility = TypeVisibility.Public,
            Partial = true
        });
        structBuilder.AddProperty(new PropertyOptions("string", "Name")
        {
            Visibility = MemberVisibility.Protected
        });
        structBuilder.EndStruct();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public partial struct DataContainer
                         {
                             protected string Name { get; set; }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_properties()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("Publicy")
        {
            Visibility = TypeVisibility.Public
        });

        classBuilder.AddProperty(new PropertyOptions("string", "Text")
        {
            Visibility = MemberVisibility.Public,
            Getter = GetterSetterVisibility.Protected,
            Setter = GetterSetterVisibility.Private,
            DefaultValue = "\"\""
        });

        classBuilder.AddProperty(new PropertyOptions("string", "Text2")
        {
            Visibility = MemberVisibility.Public,
            Getter = GetterSetterVisibility.Private,
            Setter = GetterSetterVisibility.Protected
        });

        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public class Publicy
                         {
                             public string Text { protected get; private set; } = "";
                             public string Text2 { private get; protected set; }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_fields()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("Publicy")
        {
            Visibility = TypeVisibility.Public
        });

        classBuilder.AddField(new FieldOptions("string", "Text")
        {
            Visibility = MemberVisibility.Public,
            DefaultValue = "\"\""
        });

        classBuilder.AddField(new FieldOptions("string", "Text2")
        {
            Visibility = MemberVisibility.Public
        });

        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public class Publicy
                         {
                             public string Text = "";
                             public string Text2;
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_class_with_abstract_method()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        var classBuilder = sourceBuilder.StartClass(new ClassOptions("Publicy")
        {
            Abstract = true
        });
        classBuilder.AddMethodStub(new MethodOptions("string", "Parse")
        {
            Visibility = MemberVisibility.Protected
        });
        classBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public abstract class Publicy
                         {
                             protected abstract string Parse();
                         }
                     }
                     
                     """, result);
    }

}
