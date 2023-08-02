namespace Flaeng.Productivity.Generators;

public sealed partial class FluentApiGenerator
{
    private static void Execute(SourceProductionContext context, Data source)
    {
        TryWriteDiagnostics(context, source.Diagnostics);

        var clsName = source.ClassDefinition.Name;
        if (clsName is null)
            return;

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();

        if (TryWriteNamespace(source.Namespace, builder))
            filenameParts.Add(source.Namespace!);

        // Write class and wrapper classes
        WriteWrapperClasses(source.ParentClasses, builder, filenameParts);

        builder.WriteClass(new ClassDefinition(
            visibility: source.ClassDefinition.Visibility,
            isStatic: true,
            isPartial: true,
            name: $"{source.ClassDefinition.Name}Extensions",
            typeArguments: ImmutableArray<string>.Empty,
            interfaces: ImmutableArray<InterfaceDefinition>.Empty,
            constructors: ImmutableArray<MethodDefinition>.Empty
        ));
        builder.StartScope();

        foreach (var member in source.Members)
        {
            if (member.Name is null)
                continue;

            if (member is PropertyDefinition pd && pd.SetterVisibility == null)
                continue;

            builder.WriteLine(Constants.GeneratedCodeAttribute);
            builder.WriteMethod(new MethodDefinition(
                visibility: Visibility.Public,
                isStatic: true,
                type: clsName,
                name: member.Name,
                parameters: new[]
                {
                    new MethodParameterDefinition(parameterKind: "this", type: clsName, name: "_this", defaultValue: null),
                    new MethodParameterDefinition(parameterKind: null, type: member.Type, name: member.Name, defaultValue: null)
                }.ToImmutableArray()
            ));
            builder.StartScope();
            builder.WriteLine($"_this.{member.Name} = {member.Name};");
            builder.WriteLine("return _this;");
            builder.EndScope();
        }

        string filename = filenameParts.Select(x => $"{x}.").Join() + $"{source.ClassDefinition.Name}.g.cs";
        var content = builder.Build();
        context.AddSource(filename, content);
    }
}
