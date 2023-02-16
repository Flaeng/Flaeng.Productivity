using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Flaeng.Productivity;

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

    public void StartClass(ClassOptions options) => StartType(options);
    public void StartInterface(InterfaceOptions options) => StartType(options);
    public void StartStruct(StructOptions options) => StartType(options);
    public void StartType(TypeOptions options)
    {
        builder.Append(Tabs());
        builder.Append(options.Visibility.ToString().ToLower());
        if (options.Static)
            builder.Append(" static");
        if (options.Partial)
            builder.Append(" partial");
        builder.Append(" ");
        builder.Append(options.TypeName);
        builder.Append(" ");
        builder.Append(options.Name);
        for (int i = 0; i < options.Interfaces.Length; i++)
        {
            builder.Append(i == 0 ? " : " : ", ");
            builder.Append(options.Interfaces[i]);
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

    public void AddField(FieldOptions options)
    {
        builder.Append(Tabs());
        builder.Append(options.Visibility.ToString().ToLower());
        builder.Append(" ");
        builder.Append(options.Type);
        builder.Append(" ");
        builder.Append(options.Name);
        if (options.DefaultValue != null)
        {
            builder.Append(" = ");
            builder.Append(options.DefaultValue);
        }
        builder.Append(";");
        builder.AppendLine();
    }

    public void AddProperty(PropertyOptions options)
    {
        builder.Append(Tabs());
        builder.Append(options.Visibility.ToString().ToLower());
        builder.Append(" ");
        builder.Append(options.Type);
        builder.Append(" ");
        builder.Append(options.Name);
        builder.Append(" { ");
        switch (options.Getter)
        {
            case GetterSetterVisiblity.Protected: builder.Append("protected "); break;
            case GetterSetterVisiblity.Private: builder.Append("private "); break;
        }
        builder.Append("get; ");
        switch (options.Setter)
        {
            case GetterSetterVisiblity.Protected: builder.Append("protected "); break;
            case GetterSetterVisiblity.Private: builder.Append("private "); break;
        }
        builder.Append("set; }");
        if (options.DefaultValue != null)
        {
            builder.Append(" = ");
            builder.Append(options.DefaultValue);
            builder.Append(";");
        }
        builder.AppendLine();
    }

    public void AddMethodStub(MethodOptions options)
        => StartConstructorOrMethod(options, isStub: true);

    public void StartConstructor(ConstructorOptions options)
        => StartConstructorOrMethod(options, isStub: false);

    public void StartMethod(MethodOptions options)
        => StartConstructorOrMethod(options, isStub: false);

    public void AddLineOfCode(string code)
    {
        builder.Append(Tabs());
        builder.Append(code);
        builder.AppendLine();
    }

    private void StartConstructorOrMethod(FunctionOptions options, bool isStub)
    {
        builder.Append(Tabs());

        if (options.Visibility != MemberVisiblity.None)
        {
            builder.Append(options.Visibility.ToString().ToLower());
            builder.Append(" ");
        }

        if (options is MethodOptions mo)
        {
            if (mo.Static)
                builder.Append("static ");

            if (mo.ReturnType != null)
            {
                builder.Append(mo.ReturnType);
                builder.Append(" ");
            }
        }

        builder.Append(options.Name);
        if (options.Parameters.Any())
        {
            builder.AppendLine("(");
            tabIndex++;
            foreach (var param in options.Parameters)
            {
                builder.Append(Tabs());
                builder.Append(param);
                if (param != options.Parameters.Last())
                    builder.Append(",");
                builder.AppendLine();
            }
            builder.Append(Tabs());
            builder.Append(")");
            tabIndex--;
        }
        else builder.Append("()");

        if (isStub)
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
