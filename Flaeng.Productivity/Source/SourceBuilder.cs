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

    public ClassBuilder StartClass(ClassOptions options)
    {
        StartType(options);
        return new ClassBuilder(this);
    }

    public InterfaceBuilder StartInterface(InterfaceOptions options)
    {
        StartType(options);
        return new InterfaceBuilder(this);
    }

    public StructBuilder StartStruct(StructOptions options)
    {
        StartType(options);
        return new StructBuilder(this);
    }

    public void StartType(TypeOptions options)
    {
        builder.Append(Tabs());
        builder.Append(options.Visibility.ToString().ToLower());
        if (options.Static)
            builder.Append(" static");
        if (options is ClassOptions co && co.Abstract)
            builder.Append(" abstract");
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

    public void AppendRaw(string content)
        => builder.Append(content);

    public void AppendTabs()
        => builder.Append(Tabs());

    public void AppendLineBreak()
        => builder.AppendLine();

    public void IncrementTabIndex()
        => tabIndex++;

    public void DecrementTabIndex()
        => tabIndex--;

    private string Tabs()
    {
        if (tabIndex == 0)
            return String.Empty;

        var tab = this.TabStyle == TabStyle.Tabs ? "\t" : " ";
        return String.Join("", Enumerable.Range(0, TabLength * tabIndex).Select(x => tab));
    }

    public void AddLineOfCode(string code)
    {
        builder.Append(Tabs());
        builder.Append(code);
        builder.AppendLine();
    }

    public void EndNamespace() => AppendEndTag();
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
