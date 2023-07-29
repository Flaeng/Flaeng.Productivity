namespace Flaeng.Productivity.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class InterfaceGenerator : GeneratorBase
{
    internal record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string? Namespace,
        ImmutableArray<ClassDefinition> ParentClasses,
        ClassDefinition ClassDefinition,
        ImmutableArray<IMemberDefinition> Members,
        string? InterfaceName,
        Visibility Visibility
    );

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Initialize(context, GenerateTriggerAttribute, Predicate, Transform, InterfaceDataEqualityComparer.Instance, Execute);
    }

    public static bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        if (node is not ClassDeclarationSyntax { AttributeLists.Count: > 0 } cds)
            return false;

        return HasTriggerAttribute(cds);
    }

    static readonly MethodKind[] METHOD_KINDS_TO_IGNORE = new[]
    {
        MethodKind.PropertyGet,
        MethodKind.PropertySet,
        MethodKind.Constructor
    };

    internal static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        var symbol = context.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (symbol is null)
            return default;

        var syntaxes = symbol.DeclaringSyntaxReferences.Length == 1
            ? new[] { cds }.ToImmutableArray()
            : GetAllDeclarations(symbol);

        // Make sure we only generate one new source file for partial classes in multiple files
        var triggerSyntax = syntaxes.Where(HasTriggerAttribute).First();
        if (triggerSyntax != context.Node)
            return default;

        GetAttributeParameters(triggerSyntax, out string? interfaceName, out Visibility visibility);

        List<string> usings = GetUsings(syntaxes);
        GetClassModifiers(context, out bool isPartial, out bool isStatic);

        if (isStatic)
            return DataWithDiagnostic(context, symbol, Rules.InterfaceGenerator_ClassIsStatic);

        if (isPartial == false)
            return DataWithDiagnostic(context, symbol, Rules.InterfaceGenerator_ClassIsNotPartial);

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();

        List<INamedTypeSymbol> memberContainer = new();
        INamedTypeSymbol? sym = symbol;
        do memberContainer.Add(sym);
        while ((sym = sym.BaseType) != null
            && sym.ContainingNamespace.ToString().Equals("System", StringComparison.InvariantCultureIgnoreCase) == false);

        var members = memberContainer.SelectMany(x => x.GetMembers())
            .Where(x =>
                (x is IFieldSymbol && x.IsImplicitlyDeclared == false)
                || x is IPropertySymbol
                || (
                    x is IMethodSymbol method
                    && method.IsOverride == false
                    && METHOD_KINDS_TO_IGNORE.Contains(method.MethodKind) == false
                )
            )
            .Select(x => MemberDefinitions.Parse(x, ct))
            .OfType<IMemberDefinition>()
            .ToImmutableArray();

        List<ClassDefinition> parentClasses = new();
        sym = symbol;
        while ((sym = sym.ContainingType) != null && IsSystemObjectType(sym) == false)
        {
            var cls = ClassDefinition.Parse(sym, ct);
            parentClasses.Add(cls);
        }

        var data = new Data(
            Diagnostics: new ImmutableArray<Diagnostic>(),
            Namespace: namespaceName,
            ParentClasses: parentClasses.ToImmutableArray(),
            ClassDefinition: ClassDefinition.Parse(symbol, ct),
            Members: members.ToImmutableArray(),
            interfaceName,
            visibility
        );
        return data;
    }

    private static void GetAttributeParameters(
        ClassDeclarationSyntax triggerSyntax,
        out string? interfaceName,
        out Visibility visibility
    )
    {
        Dictionary<string, string> dictionary = GetAttributeParameters(triggerSyntax, "GenerateInterface");
        if (dictionary.TryGetValue("Name", out interfaceName))
            interfaceName = interfaceName.Substring(1, interfaceName.Length - 2); // Remove quotes

        visibility = Visibility.Default;
        if (dictionary.TryGetValue("Visibility", out var visibilityString))
        {
            visibility = visibilityString.Split('.').Last() switch
            {
                "Public" => Visibility.Public,
                "Internal" => Visibility.Internal,
                _ => Visibility.Default
            };
        }
    }

    private static Data DataWithDiagnostic(
        GeneratorSyntaxContext context,
        INamedTypeSymbol symbol,
        DiagnosticDescriptor diagnosticDescriptor
    )
    {
        Data data = new();
        data.Diagnostics = new[]
        {
            Diagnostic.Create(
                descriptor: diagnosticDescriptor,
                location: context.Node.GetLocation(),
                messageArgs: new [] { symbol.Name }
            )
        }.ToImmutableArray();
        return data;
    }

    private void Execute(SourceProductionContext context, Data source)
    {
        if (source.Diagnostics != default)
        {
            foreach (var dia in source.Diagnostics)
                context.ReportDiagnostic(dia);
            return;
        }

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();

        // Get namespace
        if (source.Namespace is not null && source.Namespace.Length != 0)
        {
            builder.WriteNamespace(source.Namespace);
            builder.StartScope();
            filenameParts.Add(source.Namespace);
        }

        // Write class and wrapper classes
        foreach (var parentClass in source.ParentClasses.Reverse())
        {
            builder.WriteClass(parentClass);
            builder.StartScope();

            if (parentClass.Name is not null)
                filenameParts.Add(parentClass.Name);
        }

        // Write interface
        builder.WriteLine(Constants.GeneratedCodeAttribute);
        WriteInterface(builder, source, out var interfaceDef);
        builder.StartScope();
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

        if (classHasInterfaces == false || interfaceWithSameName.IsDefault() || interfaceWithSameName.IsPartial)
        {
            interfaceResult = new InterfaceDefinition(
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
        else
        {
            InterfaceDefinition existingInterface = data.ClassDefinition.Interfaces
                .SingleOrDefault(x => x.Name == candidate);

            string newCandidate = String.Empty;
            if (data.InterfaceName is null && existingInterface.IsDefault() == false)
            {
                for (int i = 2; i < 20; i++)
                {
                    newCandidate = $"{candidate}{i}";
                    existingInterface = data.ClassDefinition.Interfaces
                        .SingleOrDefault(x => x.Name == newCandidate);

                    if (existingInterface.IsDefault())
                        break;
                    if (existingInterface.IsPartial)
                        break;
                }
            }

            interfaceResult = existingInterface.IsDefault() == false
                ? existingInterface
                : new InterfaceDefinition(
                    visibility: Visibility.Public,
                    isPartial: false,
                    name: newCandidate,
                    typeArguments: ImmutableArray<string>.Empty,
                    members: ImmutableArray<IMemberDefinition>.Empty
                );
        }
        builder.WriteInterface(interfaceResult);
    }

    private static bool HasTriggerAttribute(ClassDeclarationSyntax syntax)
    {
        return HasAttribute(syntax, "GenerateInterface");
    }

    private static void GenerateTriggerAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("GenerateInterfaceAttribute.g.cs", SourceText.From($$"""
        // <auto-generated/>

        #nullable enable

        namespace Flaeng
        {
            {{Constants.GeneratedCodeAttribute}}
            internal enum Visibility { Default, Public, Internal }

            [global::System.AttributeUsageAttribute(
                global::System.AttributeTargets.Class,
                AllowMultiple = false,
                Inherited = false)]
            {{Constants.GeneratedCodeAttribute}}
            internal sealed class GenerateInterfaceAttribute : global::System.Attribute
            {
                public string? Name = null;
                public Visibility Visibility = Visibility.Default;
            }
        }
        """, Encoding.UTF8));
    }
}
