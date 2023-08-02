namespace Flaeng.Productivity.Generators;

public sealed partial class StartupGenerator
{
    private static readonly ClassDefinition CLASS_DEFINITION =
        new ClassDefinition(
            Visibility.Public,
            isStatic: true,
            isPartial: true,
            name: "StartupExtensions",
            typeArguments: ImmutableArray<string>.Empty,
            interfaces: ImmutableArray<InterfaceDefinition>.Empty,
            constructors: ImmutableArray<MethodDefinition>.Empty
        );

    private static MethodDefinition METHOD_DEFINITION =>
        new MethodDefinition(
            Visibility.Public,
            isStatic: true,
            type: ServiceCollectionQualifiedName,
            name: "RegisterServices",
            parameters: new[] { METHOD_PARAMETER_DEFINITION }.ToImmutableArray()
        );

    private static readonly MethodParameterDefinition METHOD_PARAMETER_DEFINITION =
        new MethodParameterDefinition(
            parameterKind: "this",
            type: ServiceCollectionQualifiedName,
            name: "services",
            defaultValue: null
        );

    private void Execute(SourceProductionContext context, Data source)
    {
        TryWriteDiagnostics(context, source.Diagnostics);

        if (source.Injectables == default || source.Injectables.Length == 0)
            return;

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();

        builder.WriteLine("using Microsoft.Extensions.DependencyInjection;");
        builder.WriteLine();

        TryWriteNamespace(source.Namespace, builder);
        builder.WriteClass(CLASS_DEFINITION);
        builder.StartScope();

        builder.WriteLine(Constants.GeneratedCodeAttribute);
        builder.WriteMethod(METHOD_DEFINITION);
        builder.StartScope();

        WriteMethodBody(source, builder);

        var content = builder.Build();
        context.AddSource("StartupExtensions.g.cs", content);
    }

    private static void WriteMethodBody(Data source, CSharpBuilder builder)
    {
        foreach (var dep in source.Injectables)
        {
            if (dep.Interfaces != default && dep.Interfaces.Length != 0)
            {
                foreach (var inter in dep.Interfaces)
                {
                    WriteRegisterPrefix(builder, dep);
                    builder.WriteLine($"<{inter}, {dep.TypeName}>();");
                }
            }
            else
            {
                WriteRegisterPrefix(builder, dep);
                builder.WriteLine($"<{dep.TypeName}>();");
            }
        }
        builder.WriteLine("return services;");
    }

    private static void WriteRegisterPrefix(CSharpBuilder builder, InjectData dep)
    {
        builder.Write("services.Add");
        switch (dep.InjectType)
        {
            case InjectType.Transient: builder.Write("Transient"); break;
            case InjectType.Scoped: builder.Write("Scoped"); break;
            case InjectType.Singleton: builder.Write("Singleton"); break;
        }
    }
}
