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

        if (options.Getter != GetterSetterVisiblity.None)
        {
            switch (options.Getter)
            {
                case GetterSetterVisiblity.Protected: builder.AppendRaw("protected "); break;
                case GetterSetterVisiblity.Private: builder.AppendRaw("private "); break;
            }
            builder.AppendRaw("get; ");
        }

        if (options.Setter != GetterSetterVisiblity.None)
        {
            switch (options.Setter)
            {
                case GetterSetterVisiblity.Protected: builder.AppendRaw("protected "); break;
                case GetterSetterVisiblity.Private: builder.AppendRaw("private "); break;
            }
            builder.AppendRaw("set; ");
        }
        builder.AppendRaw("}");

        if (options.DefaultValue != null)
        {
            builder.AppendRaw(" = ");
            builder.AppendRaw(options.DefaultValue);
            builder.AppendRaw(";");
        }
        builder.AppendLineBreak();
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

    public override string ToString()
    {
        return builder.ToString();
    }
}
