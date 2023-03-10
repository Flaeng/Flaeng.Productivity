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

        if (options.Visibility != MemberVisibility.Default)
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

        if (options.Visibility != MemberVisibility.Default)
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

    private void writeGetSetter(GetterSetterVisibility visiblity, string text)
    {
        if (visiblity == GetterSetterVisibility.None)
            return;

        switch (visiblity)
        {
            case GetterSetterVisibility.Protected: builder.AppendRaw("protected "); break;
            case GetterSetterVisibility.Private: builder.AppendRaw("private "); break;
        }
        builder.AppendRaw($"{text}; ");
    }

    protected void StartConstructorOrMethod(FunctionOptions options, bool isAbstract, bool isStub)
    {
        builder.AppendTabs();

        if (options.Visibility != MemberVisibility.Default)
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
        writeMethodParameters(options.Parameters);

        if (options is ConstructorOptions constructorOptions && constructorOptions.CallBase)
        {
            builder.AppendRaw(": base");
            writeMethodParameters(constructorOptions.BaseParameters);
            builder.AppendLineBreak();
        }

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

    private void writeMethodParameters(List<string> parameters)
    {
        if (parameters.Any())
        {
            builder.AppendRaw("(");
            builder.AppendLineBreak();
            builder.IncrementTabIndex();

            foreach (var param in parameters)
            {
                builder.AppendTabs();
                builder.AppendRaw(param);
                if (param != parameters.Last())
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
