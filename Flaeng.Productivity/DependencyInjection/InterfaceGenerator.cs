using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Flaeng.Productivity.DependencyInjection;
[Generator(LanguageNames.CSharp)]

public sealed class InterfaceGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE_NAMESPACE = "Flaeng";
    const string ATTRIBUTE_NAME = "GenerateInterfaceAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateAttribute);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider<InterfaceData>(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: 1 },
                Transform)
            .Where(static x => x.Class != null)
            .WithComparer(InterfaceDataEqualityComparer.Instance);

        context.RegisterSourceOutput<InterfaceData>(provider, Execute);
    }

    readonly static InterfaceData Default = new InterfaceData(
            null,
            new Dictionary<MemberDeclarationSyntax, ISymbol>().ToImmutableDictionary(),
            new Dictionary<MethodDeclarationSyntax, IMethodSymbol>().ToImmutableDictionary(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassData>());

    private static InterfaceData Transform(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(ctx.Node);
        if (cds.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(HasGenerateAttribute(ctx, ct))
            .Any() == false)
            return Default;

        var symbol = ctx.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (symbol == null)
            return Default;

        var interfaces = symbol.Interfaces
            .Select(x => x.Name)
            .ToImmutableArray();

        GetMemberAndMethods(
            ctx,
            cds,
            symbol,
            out Dictionary<MemberDeclarationSyntax, ISymbol> members,
            out Dictionary<MethodDeclarationSyntax, IMethodSymbol> methods,
            ct);

        var wrapperClasses = WrapperClassData.From(cds).ToImmutableArray();

        return new InterfaceData(
            cds,
            members.ToImmutableDictionary(),
            methods.ToImmutableDictionary(),
            interfaces,
            wrapperClasses);
    }

    private static void GetMemberAndMethods(
        GeneratorSyntaxContext ctx,
        ClassDeclarationSyntax cds,
        INamedTypeSymbol classSymbol,
        out Dictionary<MemberDeclarationSyntax, ISymbol> members,
        out Dictionary<MethodDeclarationSyntax, IMethodSymbol> methods,
        CancellationToken ct)
    {
        var excludeMembers = classSymbol.AllInterfaces
                    .SelectMany(x => x.GetMembers())
                    .ToImmutableArray();

        members = new();
        methods = new();

        var allMembers = new List<ISymbol>();
        var sym = classSymbol;
        do
        {
            if (sym.ToString().Equals("object") && sym.ContainingNamespace.ToString().Equals("System"))
                break;
            allMembers.AddRange(sym.GetMembers());
        }
        while ((sym = sym.BaseType) != null);

        Dictionary<MemberDeclarationSyntax, ISymbol> children = new();
        foreach (var symbol in allMembers)
        {
            var firstRefs = symbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (firstRefs == null)
                continue;

            var st = firstRefs.SyntaxTree;
            var node = firstRefs.GetSyntax();
            if (node is not MemberDeclarationSyntax)
            {
                node = node
                    .GetParents()
                    .OfType<MemberDeclarationSyntax>()
                    .FirstOrDefault();
            }
            var member = node as MemberDeclarationSyntax;
            if (member is null)
                continue;

            if (children.ContainsKey(member))
                continue;
            if (children.ContainsValue(symbol))
                continue;

            children.Add(member, symbol);
        }

        HashSet<int> methodHashcodes = new();
        foreach (var child in children)
        {
            if (child.Key.Modifiers.Any(x => x.ValueText == "public") == false)
                continue;

            if (shouldBeExcluded(ctx, child.Key, child.Value, excludeMembers, ct))
                continue;

            if (child.Key is MethodDeclarationSyntax method
                && child.Value is IMethodSymbol methodSymbol)
            {
                var hashcode = MethodDSComparer.Instance.GetHashCode(method);
                if (methodHashcodes.Contains(hashcode))
                    continue;

                methodHashcodes.Add(hashcode);
                methods.Add(method, methodSymbol);
            }
            else if (child.Key is PropertyDeclarationSyntax prop)
                members.Add(prop, child.Value);
            else if (child.Key is FieldDeclarationSyntax field)
                members.Add(field, child.Value);
        }
    }

    private static bool shouldBeExcluded(
        GeneratorSyntaxContext ctx,
        SyntaxNode node,
        ISymbol methodSymbol,
        ImmutableArray<ISymbol> excludeMembers,
        CancellationToken ct)
    {
        return
            (
                methodSymbol is IMethodSymbol mSymbol
                && excludeMembers
                    .OfType<IMethodSymbol>()
                    .Contains(mSymbol, MethodComparer.Instance)
            )
            ||
            (
                methodSymbol is IPropertySymbol pSymbol
                && excludeMembers
                    .OfType<IPropertySymbol>()
                    .Contains(pSymbol, PropertyComparer.Instance)
            )
            ||
            (
                methodSymbol is IFieldSymbol fSymbol
                && excludeMembers
                    .OfType<IFieldSymbol>()
                    .Contains(fSymbol, FieldComparer.Instance)
            );
    }

    private static Func<AttributeSyntax, bool> HasGenerateAttribute(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        return cls =>
        {
            var si = ctx.SemanticModel.GetSymbolInfo(cls, ct);
            if (si.Symbol == null)
                return false;

            return si.Symbol.ContainingType
                .ToDisplayString()
                .Equals($"{ATTRIBUTE_NAMESPACE}.{ATTRIBUTE_NAME}", StringComparison.Ordinal);
        };
    }

    private static void Execute(SourceProductionContext context, InterfaceData data)
    {
        var cls = data.Class;
        if (cls is null)
            return;
        SourceBuilder sourceBuilder = new();

        var childNodes = cls.SyntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>();

        // Add using statements
        sourceBuilder.AddUsingStatement(childNodes.Select(x => x.Name.ToString()));

        // Add namespace
        var namespaceNode = cls.Parent?.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        var isInNamespace = namespaceNode != null;
        if (namespaceNode != null)
            sourceBuilder.StartNamespace(namespaceNode.Name.ToString());

        // Add wrapper-classes
        foreach (var wrapper in data.WrapperClasses)
        {
            sourceBuilder.StartClass(new ClassOptions(wrapper.Name)
            {
                Visibility = wrapper.Visibility,
                Partial = true
            });
        }

        // Write new source code
        createInterfaceAndClass(data, cls, sourceBuilder);

        // End wrapper-classes
        for (int i = 0; i < data.WrapperClasses.Length; i++)
            sourceBuilder.AppendEndTag();

        // End namespace
        if (isInNamespace)
            sourceBuilder.EndNamespace();

        var filename = Helpers.GenerateFilename(cls, true);
        context.AddSource($"{filename}.g.cs", sourceBuilder.ToString());
    }


    private static void createInterfaceAndClass(
        InterfaceData data,
        ClassDeclarationSyntax cls,
        SourceBuilder sourceBuilder
        )
    {
        var childTokens = cls.ChildTokens();
        var className = childTokens.First(x => x.IsKind(SyntaxKind.IdentifierToken));
        var interfaceName = $"I{className}";
        if (data.InterfaceNames.Contains(interfaceName))
        {
            string tmp = interfaceName;
            int i = 2;
            do tmp = $"{interfaceName}{i++}";
            while (data.InterfaceNames.Contains(tmp));
            interfaceName = tmp;
        }

        sourceBuilder.AddGeneratedCodeAttribute();
        var visibility = TypeVisiblityHelper.GetFromTokens(childTokens);
        var interfaceBuilder = sourceBuilder.StartInterface(new InterfaceOptions(interfaceName)
        {
            Visibility = visibility,
            // Partial = true
        });

        foreach (var member in data.Members.OrderBy(x => x.Value.Name))
            writeMember(interfaceBuilder, member.Value, member.Key);

        foreach (var method in data.Methods.OrderBy(x => x.Value.Name).ThenBy(x => x.Value.Parameters.Length))
            writeMethod(interfaceBuilder, method.Value, method.Key);

        interfaceBuilder.EndInterface();

        var classBuilder = sourceBuilder.StartClass(new ClassOptions(className.ToString())
        {
            Visibility = visibility,
            Partial = true,
            Interfaces = new[] { interfaceName }
        });
        classBuilder.EndClass();
    }

    private static void writeMember(InterfaceBuilder interfaceBuilder, ISymbol symbol, CSharpSyntaxNode member)
    {
        if (member is PropertyDeclarationSyntax pds && pds.AccessorList != null)
        {
            writeMemberFromProperty(interfaceBuilder, pds);
        }
        else if (member is FieldDeclarationSyntax fds)
        {
            writeMemberFromField(interfaceBuilder, fds);
        }
    }

    private static void writeMemberFromField(InterfaceBuilder interfaceBuilder, FieldDeclarationSyntax fds)
    {
        // Interfaces cannot contain instance fields
        var isPublic = fds.Modifiers.Any(x => x.Text == "public");
        if (isPublic == false)
            return;

        var nodes = fds.Declaration.DescendantNodes();
        if (nodes.Count() < 2)
            return;

        var type = nodes.First();

        var isStatic = fds.Modifiers.Any(x => x.Text.Equals("static", StringComparison.InvariantCultureIgnoreCase));
        if (isStatic == false)
            return;

        interfaceBuilder.AddField(new FieldOptions(type.ToString(), fds.Declaration.Variables.ToString())
        {
            Static = isStatic
        });
    }

    private static void writeMemberFromProperty(InterfaceBuilder interfaceBuilder, PropertyDeclarationSyntax pds)
    {
        if (pds.AccessorList == null)
            return;

        bool publicGetter = isPublicAccessor(pds, SyntaxKind.GetAccessorDeclaration);
        bool publicSetter = isPublicAccessor(pds, SyntaxKind.SetAccessorDeclaration);

        if (publicGetter == false && publicSetter == false)
            return;

        var isStatic = pds.Modifiers.Any(x => x.Text.Equals("static", StringComparison.InvariantCultureIgnoreCase));

        var name = pds.ChildTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
            .First();

        interfaceBuilder.AddProperty(new PropertyOptions(pds.Type.ToString(), name.ToString())
        {
            Static = isStatic,
            Getter = publicGetter ? GetterSetterVisibility.Public : GetterSetterVisibility.None,
            Setter = publicSetter ? GetterSetterVisibility.Public : GetterSetterVisibility.None,
        });
    }

    private static bool isPublicAccessor(PropertyDeclarationSyntax pds, SyntaxKind kind)
    {
        if (pds.AccessorList == null)
            return false;

        var accessor = pds.AccessorList.Accessors.FirstOrDefault(x => x.IsKind(kind));
        if (accessor == null)
            return false;

        var modifier = accessor.Modifiers.FirstOrDefault();
        if (String.IsNullOrWhiteSpace(modifier.Text))
            modifier = pds.Modifiers.FirstOrDefault();

        return modifier.IsKind(SyntaxKind.PublicKeyword);
    }

    private static void writeMethod(InterfaceBuilder interfaceBuilder, IMethodSymbol symbol, MethodDeclarationSyntax method)
    {
        var name = method.ChildTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
            .FirstOrDefault();

        var isPublic = method.ChildTokens()
            .Where(x => x.IsKind(SyntaxKind.PublicKeyword))
            .Any();

        if (isPublic == false)
            return;

        var parameterList = method.DescendantNodes()
            .OfType<ParameterListSyntax>()
            .First();

        var parameters = parameterList.DescendantNodes()
            .Where(x => x.IsKind(SyntaxKind.Parameter));

        // var parameterText = parameters.Select(x => x.ToFullString());

        var returnType = TypeSymbolHelper.WriteType(symbol.ReturnType);
        var parameterText = symbol.Parameters
            .Select(TypeSymbolHelper.WriteParameter);
        // .Select(GetTypeNameAndMemberName)
        // .Select(x => $"{x.TypeName} {x.MemberName}");

        interfaceBuilder.AddMethodStub(new MethodOptions(returnType, name.Text)
        {
            Parameters = new List<string>(parameterText)
        });
    }

    private static TypeAndName GetTypeNameAndMemberName(ISymbol member)
    {
        if (member is IFieldSymbol field)
        {
            var typeName = field.Type.ContainingNamespace == null || field.Type.ContainingNamespace.ToString() == "<global namespace>"
                ? field.Type.ToString()
                : $"global::{field.Type}";
            return new TypeAndName
            {
                TypeName = typeName,
                MemberName = field.Name
            };
        }
        if (member is IPropertySymbol prop)
        {
            var typeName = prop.Type.ContainingNamespace == null || prop.Type.ContainingNamespace.ToString() == "<global namespace>"
                ? prop.Type.ToString()
                : $"global::{prop.Type}";
            return new TypeAndName
            {
                TypeName = typeName,
                MemberName = prop.Name
            };
        }
        throw new Exception("Unsupported symbol in parameterlist");
    }

    private static void GenerateAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource($"{ATTRIBUTE_NAME}.g.cs", @$"// <auto-generated/>

#nullable enable

namespace {ATTRIBUTE_NAMESPACE}
{{
    {Constants.GeneratedCodeAttribute}
    [global::System.AttributeUsageAttribute(
        global::System.AttributeTargets.Class, 
        AllowMultiple = false,
        Inherited = false)]
    internal sealed class {ATTRIBUTE_NAME} : global::System.Attribute
    {{
        public {ATTRIBUTE_NAME}()
        {{ }}
    }}
}}");
    }
}
