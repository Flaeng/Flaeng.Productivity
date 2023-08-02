namespace Flaeng.Productivity.Generators;

public sealed partial class InterfaceGenerator
{
    private void Execute(SourceProductionContext context, Data source)
    {
        if (TryWriteDiagnostics(context, source.Diagnostics))
            return;

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();

        // Get namespace
        if (TryWriteNamespace(source.Namespace, builder))
            filenameParts.Add(source.Namespace!);

        // Write class and wrapper classes
        WriteWrapperClasses(source.ParentClasses, builder, filenameParts);

        // Write interface
        builder.WriteLine(Constants.GeneratedCodeAttribute);
        WriteInterface(builder, source, out var interfaceDef);
        builder.StartScope();
        WriteMembers(source, builder, interfaceDef);
        builder.DecreaseIndentation();
        builder.WriteLine("}");

        // Write class
        builder.WriteClass(source.ClassDefinition, source.ClassDefinition.WithName(interfaceDef.Name));
        builder.WriteLine("{");
        builder.WriteLine("}");

        string filename = filenameParts.Select(x => $"{x}.").Join() + $"I{source.ClassDefinition.Name}.g.cs";
        var content = builder.Build();
        context.AddSource(filename, content);
    }

    private static void WriteMembers(Data source, CSharpBuilder builder, InterfaceDefinition interfaceDef)
    {
        foreach (var member in source.Members)
        {
            if (interfaceDef.Members.Contains(member, IMemberDefinitionEqualityComparer.Instance))
                continue;

            if (member is PropertyDefinition prop && prop.Visibility == Visibility.Public)
                builder.WriteProperty(prop);
            else if (member is FieldDefinition field && field.Visibility == Visibility.Public && field.IsStatic)
                builder.WriteField(field);
            else if (member is MethodDefinition method && method.Visibility == Visibility.Public)
                builder.WriteMethodStub(method);
        }
    }

    private void WriteInterface(
        CSharpBuilder builder,
        Data data,
        out InterfaceDefinition interfaceResult
    )
    {
        string candidate = data.InterfaceName ?? $"I{data.ClassDefinition.Name}";
        bool classHasInterfaces = data.ClassDefinition.Interfaces != default
            && data.ClassDefinition.Interfaces.Length != 0;

        var interfaceWithSameName = data.ClassDefinition.Interfaces
            .SingleOrDefault(x => x.Name == candidate);

        interfaceResult = classHasInterfaces == false || interfaceWithSameName.IsDefault() || interfaceWithSameName.IsPartial
            ? MakeInterfaceFromDefault(data, candidate, classHasInterfaces, interfaceWithSameName)
            : MakeInterfaceWithConflictingInterfaceNaming(data, candidate);

        builder.WriteInterface(interfaceResult);
    }

    private static InterfaceDefinition MakeInterfaceWithConflictingInterfaceNaming(Data data, string candidate)
    {
        InterfaceDefinition existingInterface = data.ClassDefinition.Interfaces
                        .SingleOrDefault(x => x.Name == candidate);

        string newCandidate = String.Empty;
        if (data.InterfaceName is null && existingInterface.IsDefault() == false)
        {
            newCandidate = generateNameOnConflictingInterfaceNaming(
                data,
                candidate,
                out existingInterface
            );
        }

        var interfaceResult = existingInterface.IsDefault() == false
            ? existingInterface
            : new InterfaceDefinition(
                visibility: Visibility.Public,
                isPartial: false,
                name: newCandidate,
                typeArguments: ImmutableArray<string>.Empty,
                members: ImmutableArray<IMemberDefinition>.Empty
            );
        return interfaceResult;
    }

    private static string generateNameOnConflictingInterfaceNaming(
        Data data,
        string candidate,
        out InterfaceDefinition existingInterface
    )
    {
        for (int i = 2; i < 20; i++)
        {
            var candidateName = $"{candidate}{i}";
            existingInterface = data.ClassDefinition.Interfaces
                .SingleOrDefault(x => x.Name == candidateName);

            if (existingInterface.IsDefault() || existingInterface.IsPartial)
                return candidateName;
        }
        throw new Exception("Failed to generate interfacename");
    }

    private static InterfaceDefinition MakeInterfaceFromDefault(Data data, string candidate, bool classHasInterfaces, InterfaceDefinition interfaceWithSameName)
    {
        return new InterfaceDefinition(
                        visibility: data.Visibility == Visibility.Default
                            ? data.ClassDefinition.Visibility
                            : data.Visibility,
                        isPartial: classHasInterfaces && (interfaceWithSameName.IsDefault() == false && interfaceWithSameName.IsPartial),
                        name: candidate,
                        typeArguments: data.ClassDefinition.TypeArguments,
                        members: interfaceWithSameName.IsDefault()
                            ? ImmutableArray<IMemberDefinition>.Empty
                            : interfaceWithSameName.Members
                    );
    }
}
