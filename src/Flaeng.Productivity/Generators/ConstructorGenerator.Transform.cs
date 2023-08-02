namespace Flaeng.Productivity.Generators;

public sealed partial class ConstructorGenerator : GeneratorBase
{
    internal record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        ImmutableArray<string> Usings,
        string? Namespace,
        ImmutableArray<ClassDefinition> ContainingClasses,
        ClassDefinition ClassDefinition,
        ImmutableArray<IMemberDefinition> InjectableMembers
    );

    internal static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        if (ShouldRunTransform(context, ct, out var symbol, out var syntaxes) == false)
            return new Data();

        symbol = Unsafe.As<INamedTypeSymbol>(symbol);

        List<string> usings = GetUsings(syntaxes);
        // GetClassModifiers(context, out bool isPartial, out bool isStatic);

        var classDef = ClassDefinition.Parse(symbol, ct);
        if (IsValid(classDef, out DiagnosticDescriptor? clsDiagnostics) == false && clsDiagnostics is not null)
            return DataWithDiagnostic(context, symbol, clsDiagnostics);

        List<Diagnostic> diagnostics = new();

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();

        var serializer = new SyntaxSerializer();
        ImmutableArray<IMemberDefinition> members = GetMembers(syntaxes, diagnostics, serializer);

        List<ClassDefinition> parentClasses = GetContainingTypeRecursively(symbol, ct);

        return new Data(
            Diagnostics: diagnostics.ToImmutableArray(),
            Usings: usings.Where(x => x != "using Flaeng;").Distinct().ToImmutableArray(),
            Namespace: namespaceName,
            ContainingClasses: parentClasses.ToImmutableArray(),
            ClassDefinition: classDef,
            InjectableMembers: members
        );
    }

    private static bool IsValid(ClassDefinition classDef, out DiagnosticDescriptor? diagnostics)
    {
        if (classDef.IsStatic)
        {
            diagnostics = Rules.ConstructorGenerator_ClassIsStatic;
            return false;
        }

        if (classDef.IsPartial == false)
        {
            diagnostics = Rules.ConstructorGenerator_ClassIsNotPartial;
            return false;
        }

        diagnostics = default;
        return true;
    }

    private static ImmutableArray<IMemberDefinition> GetMembers(ImmutableArray<ClassDeclarationSyntax> syntaxes, List<Diagnostic> diagnostics, SyntaxSerializer serializer)
    {
        return syntaxes
            .SelectMany(x => x.DescendantNodes(x => x is ClassDeclarationSyntax))
            .OfType<MemberDeclarationSyntax>()
            .Where(HasTriggerAttribute)
            .Where(x => x is FieldDeclarationSyntax || x is PropertyDeclarationSyntax)
            .OfType<MemberDeclarationSyntax>()
            .Select(x => new Tuple<MemberDeclarationSyntax, IMemberDefinition?>(x, serializer.Deserialize(x)))
            .Select(x => AddDiagnosticsForEachStaticMember(x.Item1, x.Item2, diagnostics))
            .OfType<IMemberDefinition>()
            .ToImmutableArray();
    }

    private static bool ShouldRunTransform(GeneratorSyntaxContext context, CancellationToken ct, out INamedTypeSymbol? symbol, out ImmutableArray<ClassDeclarationSyntax> syntaxes)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        symbol = context.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (symbol is null)
            return false;

        syntaxes = symbol.DeclaringSyntaxReferences.Length == 1
            ? new[] { cds }.ToImmutableArray()
            : GetAllDeclarations(symbol);

        // Make sure we only generate one new source file for partial classes in multiple files
        if (symbol.DeclaringSyntaxReferences[0].GetSyntax() != context.Node)
            return false;

        return true;
    }

    private static IMemberDefinition? AddDiagnosticsForEachStaticMember(MemberDeclarationSyntax syntax, IMemberDefinition? member, List<Diagnostic> diagnostics)
    {
        if (member is null || member.IsStatic == false)
            return member;

        var diag = Diagnostic.Create(
            descriptor: Rules.ConstructorGenerator_MemberIsStatic,
            location: syntax.GetLocation(),
            messageArgs: new[] { member.Name }
        );
        if (diag is not null)
            diagnostics.Add(diag);
        return null;
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

}
