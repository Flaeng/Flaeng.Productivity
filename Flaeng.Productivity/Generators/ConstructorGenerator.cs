namespace Flaeng.Productivity.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class ConstructorGenerator : GeneratorBase
{
    internal record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        ImmutableArray<string> Usings,
        string? Namespace,
        ImmutableArray<ClassDefinition> ContainingClasses,
        ClassDefinition ClassDefinition,
        ImmutableArray<IMemberDefinition> InjectableMembers
    );

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Initialize(context, GenerateTriggerAttribute, Predicate, Transform, ConstructorDataEqualityComparer.Instance, Execute);
    }

    public static bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        if (node is not ClassDeclarationSyntax { Members.Count: > 0 } cds)
            return false;

        return cds.Members.Any(HasTriggerAttribute);
    }

    internal static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        var symbol = context.SemanticModel.GetDeclaredSymbol(cds, ct) as INamedTypeSymbol;
        if (symbol is null)
            return default;

        var syntaxes = symbol.DeclaringSyntaxReferences.Length == 1
            ? new[] { cds }.ToImmutableArray()
            : GetAllDeclarations(symbol);

        // Make sure we only generate one new source file for partial classes in multiple files
        if (symbol.DeclaringSyntaxReferences[0].GetSyntax() != context.Node)
            return default;

        List<string> usings = GetUsings(syntaxes);
        GetClassModifiers(context, out bool isPartial, out bool isStatic);

        if (isStatic)
            return DataWithDiagnostic(context, symbol, Rules.ConstructorGenerator_ClassIsStatic);

        if (isPartial == false)
            return DataWithDiagnostic(context, symbol, Rules.ConstructorGenerator_ClassIsNotPartial);

        List<Diagnostic> diagnostics = new();

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();

        var members = syntaxes.SelectMany(x => x.DescendantNodes(
                    x => x is ClassDeclarationSyntax
                )
            )
            .OfType<MemberDeclarationSyntax>()
            .Where(HasTriggerAttribute)
            .Where(x =>
                x is FieldDeclarationSyntax
                || x is PropertyDeclarationSyntax
            )
            .Select(x =>
            {
                var member = MemberDefinitions.Parse(x);
                if (member is null)
                    return null;

                if (member.IsStatic)
                {
                    var diag = Diagnostic.Create(
                        descriptor: Rules.ConstructorGenerator_MemberIsStatic,
                        location: x.GetLocation(),
                        messageArgs: new[] { member.Name }
                    );
                    if (diag is not null)
                        diagnostics.Add(diag);
                    return null;
                }
                return member;
            })
            .OfType<IMemberDefinition>()
            .ToImmutableArray();

        List<ClassDefinition> parentClasses = new();
        var sym = symbol;
        while ((sym = sym.ContainingType) != null && IsSystemObjectType(sym) == false)
        {
            var cls = ClassDefinition.Parse(sym, ct);
            parentClasses.Add(cls);
        }

        return new Data(
            Diagnostics: diagnostics.ToImmutableArray(),
            Usings: usings.Where(x => x != "using Flaeng;").Distinct().ToImmutableArray(),
            Namespace: namespaceName,
            ContainingClasses: parentClasses.ToImmutableArray(),
            ClassDefinition: ClassDefinition.Parse(symbol, ct),
            InjectableMembers: members
        );
    }

    private static Data DataWithDiagnostic(
        GeneratorSyntaxContext context,
        INamedTypeSymbol symbol,
        DiagnosticDescriptor diagnosticDescriptor
    )
    {
        return new Data(
            new[] {
                    Diagnostic.Create(
                        descriptor: diagnosticDescriptor,
                        location: context.Node.GetLocation(),
                        messageArgs: new [] { symbol.Name }
                    )
            }.ToImmutableArray(),
            default,
            default,
            default,
            default,
            default
        );
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
        if (source.Namespace is not null && source.Namespace.Length != 0)
        {
            builder.WriteNamespace(source.Namespace);
            builder.StartScope();
            filenameParts.Add(source.Namespace!);
        }

        // Write class and wrapper classes
        if (source.ContainingClasses != default)
        {
            foreach (var parentClass in source.ContainingClasses.Reverse())
            {
                builder.WriteClass(parentClass.WithIsPartial(true));
                builder.StartScope();

                if (parentClass.Name is not null)
                    filenameParts.Add(parentClass.Name);
            }
        }

        // Write class itself
        builder.WriteClass(source.ClassDefinition.WithIsPartial(true));
        builder.StartScope();

        // Writing constructor
        builder.WriteLine(Constants.GeneratedCodeAttribute);
        builder.WriteLine($"public {source.ClassDefinition.Name}(", increaseIndentation: true);
        // Write constructor parameters
        List<string> injectedList = new();
        if (source.InjectableMembers != default)
        {
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
        builder.DecreaseIndentation();
        builder.WriteLine(")");

        // Write constructor body
        builder.StartScope();
        if (source.InjectableMembers != default)
        {
            foreach (var member in source.InjectableMembers)
            {
                string? name = member is IHasPrettyName pretty ? pretty.GetPrettyName() : member.Name;
                name ??= member.Name;
                if (name is not null)
                    builder.WriteLine($"this.{member.Name} = {name};");
            }
        }
        builder.EndScope();

        string filename = filenameParts.Select(x => $"{x}.").Join() + $"{source.ClassDefinition.Name}.g.cs";
        var content = builder.Build();
        context.AddSource(filename, content);
    }

    private static bool HasTriggerAttribute(MemberDeclarationSyntax syntax)
    {
        return HasAttribute(syntax, "Inject");
    }

    private static void GenerateTriggerAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("InjectAttribute.g.cs", SourceText.From($$"""
        // <auto-generated/>

        #nullable enable

        namespace Flaeng
        {
            [global::System.AttributeUsageAttribute(
                global::System.AttributeTargets.Property | global::System.AttributeTargets.Field, 
                AllowMultiple = false, 
                Inherited = false)]
            {{Constants.GeneratedCodeAttribute}}
            internal sealed class InjectAttribute : global::System.Attribute
            {
            }
        }
        """, Encoding.UTF8));
    }
}
