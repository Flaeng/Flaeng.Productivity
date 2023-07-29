namespace Flaeng.Productivity.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class StartupGenerator : GeneratorBase
{
    private const string ServiceCollectionQualifiedName = "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

    public enum InjectType { Transient, Scoped, Singleton }

    public record struct InjectData(InjectType InjectType, string TypeName, ImmutableArray<string> Interfaces);

    public record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string Namespace,
        ImmutableArray<InjectData> Injectables
    );

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateDependencies);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider<Data>(Predicate, Transform)
            .Where(static x => x != null)
            ;
        // .WithComparer(ConstructorDataEqualityComparer.Instance);

        context.RegisterSourceOutput(provider, Execute);
    }

    public static bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        if (node is not ClassDeclarationSyntax { AttributeLists.Count: > 0 } cds)
            return false;

        var result = HasTriggerAttribute(cds);
        return result;
    }

    public static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        var namespaceName = context.SemanticModel.Compilation.GlobalNamespace.Name;

        var classDeclarations = context.SemanticModel.Compilation.SyntaxTrees
            .Select(x => x.GetRoot(ct))
            .SelectMany(x => x.DescendantNodesAndSelf(x =>
                x is ClassDeclarationSyntax 
                || x is NamespaceDeclarationSyntax 
                || x is FileScopedNamespaceDeclarationSyntax 
                || x is CompilationUnitSyntax
            ))
            .OfType<ClassDeclarationSyntax>()
            .Where(HasTriggerAttribute)
            .ToImmutableArray();

        if (classDeclarations.First() != context.Node)
            return default;

        var injectables = classDeclarations
            .Select(x => TryGetDeclaredSymbol(context, x, ct))
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>()
            .Select(x => ToInjectData(x, ct))
            .ToImmutableArray();

        return new Data(
            Diagnostics: ImmutableArray<Diagnostic>.Empty,
            Namespace: namespaceName,
            Injectables: injectables
        );
    }

    // Has poor performance due to try-catch and looping through all syntax trees
    // TODO: Optimize this method
    private static INamedTypeSymbol? TryGetDeclaredSymbol(GeneratorSyntaxContext context, ClassDeclarationSyntax syntax, CancellationToken ct)
    {
        try
        {
            return context.SemanticModel.GetDeclaredSymbol(syntax, ct);
        }
        catch
        {
            foreach (var tree in context.SemanticModel.Compilation.SyntaxTrees)
            {
                var treeModel = context.SemanticModel.Compilation.GetSemanticModel(tree);
                try
                {
                    return treeModel.GetDeclaredSymbol(syntax, ct);;
                }
                catch
                { }
            }
            return null;
        }
    }

    private static InjectData ToInjectData(INamedTypeSymbol symbol, CancellationToken ct)
    {
        static string FormatName(ISymbol sym) 
            => sym.ContainingNamespace.IsGlobalNamespace 
                ? $"global::{sym.Name}" 
                : $"global::{sym.ContainingNamespace}.{sym.Name}";

        return new InjectData(
            GetInjectType(symbol, ct),
            FormatName(symbol),
            symbol.Interfaces.Select(FormatName).ToImmutableArray()
        );
    }

    private static Data DataWithDiagnostic(
        GeneratorSyntaxContext context,
        INamedTypeSymbol symbol,
        DiagnosticDescriptor diagnosticDescriptor
    )
    {
        var data = new Data();
        // data.Diagnostics = new Diagnostic[] {
        //     Diagnostic.Create()
        // }.ToImmutableArray();
        return data;
    }

    private void Execute(SourceProductionContext context, Data source)
    {
        if (source == default)
            return;

        if (source.Diagnostics != default && source.Diagnostics.Length != 0)
        {
            foreach (var dia in source.Diagnostics)
                context.ReportDiagnostic(dia);
        }

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();

        builder.WriteLine("using Microsoft.Extensions.DependencyInjection;");
        builder.WriteLine();

        if (source.Namespace is not null && source.Namespace.Length != 0)
        {
            builder.WriteNamespace(source.Namespace);
            builder.StartScope();
        }

        builder.WriteClass(new ClassDefinition(
            Visibility.Public,
            isStatic: true,
            isPartial: true,
            name: "StartupExtensions",
            typeArguments: ImmutableArray<string>.Empty,
            interfaces: ImmutableArray<InterfaceDefinition>.Empty,
            constructors: ImmutableArray<MethodDefinition>.Empty
        ));
        builder.StartScope();

        builder.WriteLine(Constants.GeneratedCodeAttribute);
        builder.WriteMethod(new MethodDefinition(
            Visibility.Public,
            isStatic: true,
            type: ServiceCollectionQualifiedName,
            name: "RegisterServices",
            new [] { 
                new MethodParameterDefinition(
                    parameterKind: "this",
                    type: ServiceCollectionQualifiedName,
                    name: "services",
                    defaultValue: null
                )
            }.ToImmutableArray()
        ));
        builder.StartScope();

        foreach (var dep in source.Injectables)
        {
            if (dep.Interfaces != default && dep.Interfaces.Length != 0)
            {
                foreach (var inter in dep.Interfaces)
                {
                    builder.Write("services.Add");
                    switch (dep.InjectType)
                    {
                        case InjectType.Transient: builder.Write("Transient"); break;
                        case InjectType.Scoped: builder.Write("Scoped"); break;
                        case InjectType.Singleton: builder.Write("Singleton"); break;
                    }
                    builder.WriteLine($"<{inter}, {dep.TypeName}>();");
                }
            }
            else 
            {
                builder.Write("services.Add");
                switch (dep.InjectType)
                {
                    case InjectType.Transient: builder.Write("Transient"); break;
                    case InjectType.Scoped: builder.Write("Scoped"); break;
                    case InjectType.Singleton: builder.Write("Singleton"); break;
                }
                builder.WriteLine($"<{dep.TypeName}>();");
            }
        }
        builder.WriteLine("return services;");

        var content = builder.Build();
        context.AddSource("StartupExtensions.g.cs", content);
    }

    private static bool HasTriggerAttribute(ClassDeclarationSyntax syntax)
    {
        return HasAttribute(syntax, "RegisterService");
    }

    private static InjectType GetInjectType(ISymbol symbol, CancellationToken ct)
    {
        var clsDeclarations = symbol.DeclaringSyntaxReferences
            .Select(x => x.GetSyntax(ct))
            .OfType<ClassDeclarationSyntax>();

        var clsDeclarationWithAttr = clsDeclarations
            .Where(HasTriggerAttribute)
            .Single();

        var dictionary = GetAttributeParameters(clsDeclarationWithAttr, "RegisterService");
        var item = dictionary.SingleOrDefault();
        return item.Value?.Split('.')?.Last() switch
        {
            "Transient" => InjectType.Transient,
            "Scoped" => InjectType.Scoped,
            "Singleton" => InjectType.Singleton,
            _ => InjectType.Scoped,
        };
    }

    private void GenerateDependencies(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            "RegisterAttribute.g.cs", 
            SourceText.From($$"""
            // <auto-generated/>

            #nullable enable

            namespace Flaeng
            {
                {{Constants.GeneratedCodeAttribute}}
                internal enum ServiceType { Transient, Scoped, Singleton }

                [global::System.AttributeUsageAttribute(
                    global::System.AttributeTargets.Class,
                    AllowMultiple = false, 
                    Inherited = false)]
                {{Constants.GeneratedCodeAttribute}}
                internal sealed class RegisterServiceAttribute : global::System.Attribute
                {
                    public ServiceType ServiceType = ServiceType.Scoped;
                }
            }
            """, Encoding.UTF8)
        );
    }
}