using System.Linq;

namespace Flaeng.Productivity;

abstract class BaseBuilder
{
    protected SourceBuilder builder;

    public BaseBuilder(SourceBuilder builder)
    {
        this.builder = builder;
    }

    public void EndMethod() => builder.AppendEndTag();

    public void AddField(FieldOptions options)
    {
        builder.AppendTabs();

        if (options.Visibility != MemberVisiblity.None)
        {
            builder.AppendRaw(options.Visibility.ToString().ToLower());
            builder.AppendRaw(" ");
        }

        if (options.Static)
            builder.AppendRaw("static ");

        builder.AppendRaw(options.Type);
        builder.AppendRaw(" ");
        builder.AppendRaw(options.Name);
        if (options.DefaultValue != null)
        {
            builder.AppendRaw(" = ");
            builder.AppendRaw(options.DefaultValue);
        }
        builder.AppendRaw(";");
        builder.AppendLineBreak();
    }

    public void AddProperty(PropertyOptions options)
    {
        builder.AppendTabs();

        if (options.Visibility != MemberVisiblity.None)
        {
            builder.AppendRaw(options.Visibility.ToString().ToLower());
            builder.AppendRaw(" ");
        }

        if (options.Static)
            builder.AppendRaw("static ");

        builder.AppendRaw(options.Type);
        builder.AppendRaw(" ");
        builder.AppendRaw(options.Name);
        builder.AppendRaw(" { ");

        writeGetSetter(options.Getter, "get");
        writeGetSetter(options.Setter, "set");

        builder.AppendRaw("}");

        if (options.DefaultValue != null)
        {
            builder.AppendRaw(" = ");
            builder.AppendRaw(options.DefaultValue);
            builder.AppendRaw(";");
        }
        builder.AppendLineBreak();
    }

    private void writeGetSetter(GetterSetterVisiblity visiblity, string text)
    {
        if (visiblity == GetterSetterVisiblity.None)
            return;

        switch (visiblity)
        {
            case GetterSetterVisiblity.Protected: builder.AppendRaw("protected "); break;
            case GetterSetterVisiblity.Private: builder.AppendRaw("private "); break;
        }
        builder.AppendRaw($"{text}; ");
    }

    protected void StartConstructorOrMethod(FunctionOptions options, bool isAbstract, bool isStub)
    {
        builder.AppendTabs();

        if (options.Visibility != MemberVisiblity.None)
        {
            builder.AppendRaw(options.Visibility.ToString().ToLower());
            builder.AppendRaw(" ");
        }

        if (isAbstract)
            builder.AppendRaw("abstract ");

        if (options is MethodOptions mo)
        {
            if (mo.Static)
                builder.AppendRaw("static ");

            if (mo.ReturnType != null)
            {
                builder.AppendRaw(mo.ReturnType);
                builder.AppendRaw(" ");
            }
        }

        builder.AppendRaw(options.Name);
        writeMethodParameters(options);
        writeMethodBody(isStub);
    }

    private void writeMethodBody(bool isStub)
    {
        if (isStub)
        {
            builder.AppendRaw(";");
            builder.AppendLineBreak();
        }
        else
        {
            builder.AppendLineBreak();
            builder.AppendTabs();
            builder.AppendRaw("{");
            builder.AppendLineBreak();
            builder.IncrementTabIndex();
        }
    }

    private void writeMethodParameters(FunctionOptions options)
    {
        if (options.Parameters.Any())
        {
            builder.AppendRaw("(");
            builder.AppendLineBreak();
            builder.IncrementTabIndex();

            foreach (var param in options.Parameters)
            {
                builder.AppendTabs();
                builder.AppendRaw(param);
                if (param != options.Parameters.Last())
                    builder.AppendRaw(",");
                builder.AppendLineBreak();
            }
            builder.AppendTabs();
            builder.AppendRaw(")");
            builder.DecrementTabIndex();
        }
        else builder.AppendRaw("()");
    }

    public void AddLineOfCode(string code)
    {
        builder.AppendTabs();
        builder.AppendRaw(code);
        builder.AppendLineBreak();
    }
}
