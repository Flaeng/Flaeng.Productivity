using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Flaeng.Productivity;

internal enum TypeVisiblity { Public, Internal, Private }
internal enum MemberVisiblity { Public, Internal, Protected, Private }
internal enum GetterSetterVisiblity { Inherited, Public, Internal, Protected, Private, None }
internal enum TabStyle { Tabs, Spaces }
internal class SourceBuilder
{
    private readonly List<string> usings = new();
    private readonly StringBuilder builder = new();
    public bool NullableEnable { get; set; } = true;
    private int tabIndex = 0;
    public TabStyle TabStyle = TabStyle.Spaces;
    public int TabLength = 4;

    public void AddUsingStatement(string @namespace)
        => usings.Add($"using {@namespace};");

    public void AddUsingStatement(IEnumerable<string> namespaces)
    {
        foreach (var item in namespaces)
            AddUsingStatement(item);
    }

    public void StartNamespace(string name)
    {
        AddLineOfCode($"namespace {name}");
        AddLineOfCode("{");
        tabIndex++;
    }

    public void StartClass(
        TypeVisiblity visibility,
        string name,
        bool @static = false,
        bool partial = false,
        string[]? interfaces = null
        )
        => StartType(visibility, @static, partial, "class", name, interfaces ?? new string[0]);

    public void StartInterface(TypeVisiblity visibility, string name, bool partial = false)
        => StartType(visibility, false, partial, "interface", name, new string[0]);

    public void StartInterface(TypeVisiblity visibility, string name, bool partial, string[] interfaces)
        => StartType(visibility, false, partial, "interface", name, interfaces);

    public void StartStruct(TypeVisiblity visibility, string name, bool partial = false)
        => StartType(visibility, false, partial, "struct", name, new string[0]);

    public void StartType(
        TypeVisiblity visibility,
        bool @static, 
        bool partial,
        string typeName,
        string name,
        string[] interfaces
        )
    {
        builder.Append(Tabs());
        builder.Append(visibility.ToString().ToLower());
        if (@static)
            builder.Append(" static");
        if (partial)
            builder.Append(" partial");
        builder.Append(" ");
        builder.Append(typeName);
        builder.Append(" ");
        builder.Append(name);
        for (int i = 0; i < interfaces.Length; i++)
        {
            builder.Append(i == 0 ? " : " : ", ");
            builder.Append(interfaces[i]);
        }
        builder.AppendLine();

        builder.Append(Tabs());
        builder.AppendLine("{");
        tabIndex++;
    }

    private string Tabs()
    {
        if (tabIndex == 0)
            return String.Empty;

        var tab = this.TabStyle == TabStyle.Tabs ? "\t" : " ";
        return String.Join("", Enumerable.Range(0, TabLength * tabIndex).Select(x => tab));
    }

    public void AddProperty(
        MemberVisiblity visibility,
        string propertyType,
        string propertyName,
        GetterSetterVisiblity getter = GetterSetterVisiblity.Inherited,
        GetterSetterVisiblity setter = GetterSetterVisiblity.Inherited,
        string? defaultValue = null
        )
    {
        builder.Append(Tabs());
        builder.Append(visibility.ToString().ToLower());
        builder.Append(" ");
        builder.Append(propertyType);
        builder.Append(" ");
        builder.Append(propertyName);
        builder.Append(" { ");
        switch (getter)
        {
            case GetterSetterVisiblity.Protected: builder.Append("protected "); break;
            case GetterSetterVisiblity.Private: builder.Append("private "); break;
        }
        builder.Append("get; ");
        switch (setter)
        {
            case GetterSetterVisiblity.Protected: builder.Append("protected "); break;
            case GetterSetterVisiblity.Private: builder.Append("private "); break;
        }
        builder.Append("set; }");
        if (defaultValue != null)
        {
            builder.Append(" = ");
            builder.Append(defaultValue);
            builder.Append(";");
        }
        builder.AppendLine();
    }

    public void StartConstructor(
        MemberVisiblity visibility,
        string name,
        IEnumerable<string> parameters
        )
    {
        StartConstructorOrMethod(visibility, false, null, name, parameters);
    }

    public void DeclareMethod(
        string returnType,
        string name,
        IEnumerable<string> parameters,
        bool @static = false
        )
    {
        StartConstructorOrMethod(null, @static, returnType, name, parameters, autoClose: true);
    }

    public void StartMethod(
        MemberVisiblity visibility,
        string returnType,
        string name,
        IEnumerable<string> parameters,
        bool @static = false
        )
    {
        StartConstructorOrMethod(visibility, @static, returnType, name, parameters);
    }

    public void AddLineOfCode(string code)
    {
        builder.Append(Tabs());
        builder.Append(code);
        builder.AppendLine();
    }

    private void StartConstructorOrMethod(
        MemberVisiblity? visibility,
        bool @static,
        string? returnType,
        string name,
        IEnumerable<string> parameters,
        bool autoClose = false
        )
    {
        builder.Append(Tabs());
        if (visibility != null)
        {
            builder.Append(visibility.ToString().ToLower());
            builder.Append(" ");
        }
        if (@static)
            builder.Append("static ");
        if (returnType != null)
        {
            builder.Append(returnType);
            builder.Append(" ");
        }
        builder.Append(name);
        if (parameters.Any())
        {
            builder.AppendLine("(");
            tabIndex++;
            foreach (var param in parameters)
            {
                builder.Append(Tabs());
                builder.Append(param);
                if (param != parameters.Last())
                    builder.Append(",");
                builder.AppendLine();
            }
            builder.Append(Tabs());
            builder.Append(")");
            tabIndex--;
        }
        else builder.Append("()");

        if (autoClose)
        {
            builder.AppendLine(";");
        }
        else
        {
            builder.AppendLine();
            builder.Append(Tabs());
            builder.AppendLine("{");
            tabIndex++;
        }
    }

    public void EndNamespace() => AppendEndTag();
    public void EndClass() => AppendEndTag();
    public void EndInterface() => AppendEndTag();
    public void EndStruct() => AppendEndTag();
    public void EndConstructor() => AppendEndTag();
    public void EndMethod() => AppendEndTag();
    public void AppendEndTag()
    {
        tabIndex--;
        builder.Append(Tabs());
        builder.AppendLine("}");
    }

    public Microsoft.CodeAnalysis.Text.SourceText Build()
        => Microsoft.CodeAnalysis.Text.SourceText.From(this.ToString());

    public override string ToString()
    {
        StringBuilder result = new();
        result.AppendLine("// <auto-generated/>");
        result.AppendLine();
        if (usings.Any())
        {
            foreach (var u in usings.Distinct())
                result.AppendLine(u);
            result.AppendLine();
        }
        
        result.Append($"#nullable {(NullableEnable ? "enable" : "disable")}");
        
        if (builder.Length != 0)
        {
            result.AppendLine();
            result.AppendLine();
            result.Append(builder.ToString());
        }
        
        return result.ToString();
    }

    public void AddGeneratedCodeAttribute()
    {
        var ass = Assembly.GetExecutingAssembly();
        AddLineOfCode(@$"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ass.GetName().Name}"", ""{ass.GetName().Version}"")]");
    }
}