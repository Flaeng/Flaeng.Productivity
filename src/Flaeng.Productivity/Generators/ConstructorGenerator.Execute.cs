namespace Flaeng.Productivity.Generators;

public sealed partial class ConstructorGenerator : GeneratorBase
{
    private void Execute(SourceProductionContext context, Data source)
    {
        TryWriteDiagnostics(context, source.Diagnostics);

        if (source.ClassDefinition.Name == default)
            return;

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();

        if (source.Usings != default && source.Usings.Length != 0)
        {
            foreach (var namespaceLine in source.Usings)
                builder.WriteLine(namespaceLine);
            builder.WriteLine();
        }

        // Get namespace
        if (TryWriteNamespace(source.Namespace, builder))
            filenameParts.Add(source.Namespace!);

        // Write class and wrapper classes
        WriteContainingClasses(source, builder, filenameParts);

        // Write class itself
        builder.WriteClass(source.ClassDefinition.WithIsPartial(true));
        builder.StartScope();

        WriteConstructorMethod(source, builder);

        string filename = filenameParts.Select(x => $"{x}.").Join() + $"{source.ClassDefinition.Name}.g.cs";
        var content = builder.Build();
        context.AddSource(filename, content);
    }

    private static void WriteConstructorMethod(Data source, CSharpBuilder builder)
    {
        // Writing constructor
        builder.WriteLine(Constants.GeneratedCodeAttribute);
        builder.WriteLine($"public {source.ClassDefinition.Name}(", increaseIndentation: true);
        // Write constructor parameters
        WriteConstructorParameters(source, builder);
        builder.DecreaseIndentation();
        builder.WriteLine(")");

        // Write constructor body
        builder.StartScope();
        WriteConstructorBody(source, builder);
        builder.EndScope();
    }

    private static void WriteContainingClasses(Data source, CSharpBuilder builder, List<string> filenameParts)
    {
        if (source.ContainingClasses == default || source.ContainingClasses.Length == 0)
            return;

        foreach (var parentClass in source.ContainingClasses.Reverse())
        {
            builder.WriteClass(parentClass.WithIsPartial(true));
            builder.StartScope();

            if (parentClass.Name is not null)
                filenameParts.Add(parentClass.Name);
        }
    }

    private static void WriteConstructorBody(Data source, CSharpBuilder builder)
    {
        if (source.InjectableMembers == default || source.InjectableMembers.Length == 0)
            return;

        foreach (var member in source.InjectableMembers)
        {
            string? name = member is IHasPrettyName pretty ? pretty.GetPrettyName() : member.Name;
            name ??= member.Name;
            if (name is not null)
                builder.WriteLine($"this.{member.Name} = {name};");
        }
    }

    private static void WriteConstructorParameters(Data source, CSharpBuilder builder)
    {
        if (source.InjectableMembers == default || source.InjectableMembers.Length == 0)
            return;

        for (int i = 0; i < source.InjectableMembers.Length; i++)
        {
            var member = source.InjectableMembers[i];

            string? name = member is IHasPrettyName pretty ? pretty.GetPrettyName() : member.Name;
            name ??= member.Name;
            if (name is null)
                continue;

            builder.Write($"{member.Type} {name}");

            if (i + 1 != source.InjectableMembers.Length)
                builder.Write(",");

            builder.WriteLine();
        }
    }

}
