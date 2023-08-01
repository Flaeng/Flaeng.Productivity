namespace Flaeng.Productivity.SourceBuilders;

internal record struct CSharpOptions(
    bool IgnoreUnnecessaryUsingDirectives
);

internal class CSharpBuilder : SourceBuilder
{
    public CSharpBuilder(CSharpOptions options) : base()
    {
        bool hasAnyPragma = false;
        if (options.IgnoreUnnecessaryUsingDirectives)
        {
            WriteLine("#pragma warning disable CS8019");
            hasAnyPragma = true;
        }
        if (hasAnyPragma)
            WriteLine();
    }

    public void WriteNamespace(string namespaceName)
    {
        base.WriteLine($"namespace {namespaceName}");
    }

    private void WriteClassName(ClassDefinition cls)
    {
        if (cls.Name is null)
            throw new NullReferenceException();

        base.Write(cls.Name);
        if (cls.TypeArguments == default || cls.TypeArguments.Length == 0)
            return;

        base.Write("<");
        for (int i = 0; i < cls.TypeArguments.Length; i++)
        {
            var item = cls.TypeArguments[i];
            base.Write(item);
            if (i + 1 != cls.TypeArguments.Length)
                base.Write(", ");
        }
        base.Write(">");
    }

    public void WriteInterface(InterfaceDefinition interfaceResult)
    {
        WriteVisibility(interfaceResult.Visibility);
        WriteIf(interfaceResult.IsPartial, "partial ");
        base.Write("interface ");

        base.Write(interfaceResult.Name);

        if (interfaceResult.TypeArguments != default && interfaceResult.TypeArguments.Length != 0)
        {
            base.Write("<");
            for (int i = 0; i < interfaceResult.TypeArguments.Length; i++)
            {
                var item = interfaceResult.TypeArguments[i];
                base.Write(item);
                if (i + 1 != interfaceResult.TypeArguments.Length)
                    base.Write(", ");
            }
            base.Write(">");
        }

        base.WriteLine();
    }

    public void WriteProperty(PropertyDefinition prop)
    {
        if (prop.IsStatic)
            base.Write("static ");

        base.Write($"{prop.Type} {prop.Name} {{");

        if (prop.GetterVisibility == Visibility.Default || prop.GetterVisibility == Visibility.Public)
            base.Write($" get;");

        if (prop.SetterVisibility == Visibility.Default || prop.SetterVisibility == Visibility.Public)
            base.Write($" set;");

        base.Write($" }}");

        if (prop.IsStatic && TryWriteDefaultValue(prop))
            base.Write($";");

        base.WriteLine();
    }

    private bool TryWriteDefaultValue(IHasDefaultValue obj)
    {
        if (obj.DefaultValue is null)
            return false;

        base.Write(" = ");
        base.Write(obj.DefaultValue);
        return true;
    }

    public void StartScope()
    {
        base.WriteLine("{", increaseIndentation: true);
    }

    public void EndScope()
    {
        base.DecreaseIndentation();
        base.WriteLine("}");
    }

    public void WriteField(FieldDefinition field)
    {
        WriteIf(field.IsStatic, "static ");

        base.Write($"{field.Type} {field.Name}");

        TryWriteDefaultValue(field);

        base.Write(";");
        base.WriteLine();
    }

    private void WriteIf(bool predicate, string text)
    {
        if (predicate == false)
            return;
        Write(text);
    }

    public void WriteMethod(MethodDefinition method)
    {
        WriteMethodSignature(method, isStub: false);
    }

    private void WriteMethodSignature(MethodDefinition method, bool isStub)
    {
        if (isStub == false)
        {
            WriteVisibility(method.Visibility);
            WriteIf(method.IsStatic, "static ");
        }
        else
        {
            WriteIf(method.IsStatic, "static abstract ");
        }

        if (method.Parameters == default || method.Parameters.Length == 0)
        {
            base.WriteLine($"{method.Type} {method.Name}();");
            return;
        }

        base.WriteLine($"{method.Type} {method.Name}(", increaseIndentation: true);
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            var param = method.Parameters[i];
            WriteMethodParameter(param);
            if (i + 1 < method.Parameters.Length)
                base.Write(",");
            base.WriteLine();
        }
        base.DecreaseIndentation();
        base.Write(")");
        WriteIf(isStub, ";");
        base.WriteLine();
    }

    public void WriteMethodStub(MethodDefinition method)
    {
        WriteMethodSignature(method, isStub: true);
    }

    private void WriteMethodParameter(MethodParameterDefinition source)
    {
        if (source.Type is null || source.Name is null)
            throw new NullReferenceException();

        if (source.ParameterKind is not null)
        {
            base.Write(source.ParameterKind);
            base.Write(" ");
        }

        base.Write(source.Type);
        base.Write(" ");
        base.Write(source.Name);

        if (source.DefaultValue is not null)
        {
            base.Write(" = ");
            base.Write(source.DefaultValue);
        }
    }

    public void WriteClass(ClassDefinition cls, ClassDefinition? intface = null)
    {
        WriteVisibility(cls.Visibility);
        WriteIf(cls.IsStatic, "static ");
        WriteIf(cls.IsPartial, "partial ");
        base.Write("class ");
        WriteClassName(cls);
        if (intface is not null)
        {
            base.Write(" : ");
            WriteClassName(intface.Value);
        }
        base.WriteLine();
    }

    protected void WriteVisibility(Visibility visibility)
    {
        if (visibility == Visibility.Default)
            return;

        switch (visibility)
        {
            case Visibility.Public: base.Write("public"); break;
            case Visibility.Internal: base.Write("internal"); break;
            case Visibility.Private: base.Write("private"); break;
        }
        base.Write(" ");
    }

}
