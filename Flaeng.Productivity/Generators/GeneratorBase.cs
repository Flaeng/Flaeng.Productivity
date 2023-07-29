namespace Flaeng.Productivity.Generators;

public abstract class GeneratorBase : IIncrementalGenerator
{
    internal static readonly CSharpOptions DefaultCSharpOptions = new(IgnoreUnnecessaryUsingDirectives: true);
    public abstract void Initialize(IncrementalGeneratorInitializationContext context);

    protected static bool IsSystemObjectType(INamedTypeSymbol? sym)
    {
        if (sym is null)
            return false;

        return sym.ContainingNamespace.ToDisplayString().Equals("System", StringComparison.InvariantCultureIgnoreCase)
            && sym.ToDisplayString().Equals("object", StringComparison.InvariantCultureIgnoreCase);
    }

    protected static Dictionary<string, string> GetAttributeParameters(
        ClassDeclarationSyntax syntax,
        string attributeName
        )
    {
        var attrList = syntax
            .ChildNodes()
            .OfType<AttributeListSyntax>();

        var attribute = attrList
            .SelectMany(x => x.ChildNodes())
            .OfType<AttributeSyntax>()
            .Single(x =>
            {
                var qualifiedName = x.ChildNodes().OfType<NameSyntax>().First();
                return NameSyntaxIsAttribute(qualifiedName, attributeName);
            });

        var attrArgumentListSyntaxes = attribute
            .ChildNodes()
            .OfType<AttributeArgumentListSyntax>()
            .SingleOrDefault();

        if (attrArgumentListSyntaxes is null)
            return new();

        var attrArgumentSyntaxes = attrArgumentListSyntaxes
            .ChildNodes()
            .OfType<AttributeArgumentSyntax>();

        return attrArgumentSyntaxes
            .Select(x => x.ToString())
            .Select(x => x.Split('='))
            .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
    }

    protected static void GetClassModifiers(GeneratorSyntaxContext context, out bool isPartial, out bool isStatic)
    {
        isPartial = false;
        isStatic = false;
        foreach (var child in context.Node.ChildNodesAndTokens())
        {
            switch (child.RawKind)
            {
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;

                case (int)SyntaxKind.PartialKeyword:
                    isPartial = true;
                    break;
            }
        }
    }

    protected static bool HasAttribute(MemberDeclarationSyntax syntax, string attrName)
    {
        return syntax.AttributeLists
            .SelectMany(x => x.Attributes)
            .Any(attr =>
            {
                var qualifiedName = attr.ChildNodes().OfType<NameSyntax>().First();
                bool result = NameSyntaxIsAttribute(qualifiedName, attrName);
                return result;
            });
    }

    private static bool NameSyntaxIsAttribute(NameSyntax qualifiedName, string attrName)
    {
        var name = qualifiedName.ToString();
        
        // If user uses normal usings 
        return name == attrName
            || name == $"{attrName}Attribute"
            // If user uses name with namespace 
            || name == $"Flaeng.{attrName}"
            || name == $"Flaeng.{attrName}Attribute"
            // If user uses fully qualified name 
            || name == $"global::Flaeng.{attrName}"
            || name == $"global::Flaeng.{attrName}Attribute"
            // If user uses using-alias 
            || name.Split('.').Last() == attrName
            || name.Split('.').Last() == $"{attrName}Attribute";
    }

    protected static List<string> GetUsings(ImmutableArray<SyntaxNode> roots)
    {
        List<string> usings = new();
        foreach (var child in roots.SelectMany(x => x.ChildNodesAndTokens()))
        {
            switch (child.RawKind)
            {
                case (int)SyntaxKind.UsingDirective:
                    usings.Add(child.ToString());
                    break;
                case (int)SyntaxKind.UsingKeyword:
                    Debugger.Break();
                    usings.Add(child.ToString());
                    break;
                case (int)SyntaxKind.UsingStatement:
                    Debugger.Break();
                    usings.Add(child.ToString());
                    break;
            }
        }
        return usings;
    }

    private static string GetTypeStringFromSymbol(ISymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arrayType)
        {
            var nSpace = arrayType.ContainingNamespace;
            var cType = arrayType.ContainingType;
            return $"{GetTypeStringFromSymbol(arrayType.ElementType)}[]";
        }

        if (symbol is not INamedTypeSymbol namedType)
            throw new Exception("GetTypeStringFromSymbol was passed not INamedTypeSymbol");

        if (namedType.IsGenericType == false)
            return $"global::{symbol.ContainingNamespace}.{symbol.Name}";

        List<string> typeArgs = new();
        foreach (var typeArg in namedType.TypeArguments)
        {
            var typeString = (typeArg is ITypeParameterSymbol tps)
                ? tps.Name
                : GetTypeStringFromSymbol(typeArg);
            typeArgs.Add(typeString);
        }

        return $"global::{symbol.ContainingNamespace}.{symbol.Name}<{(String.Join(", ", typeArgs))}>";
    }
}
