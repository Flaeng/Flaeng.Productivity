using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity;

public class TypeAndName
{
    public string TypeName { get; private set; } = "";
    public string Name { get; private set; } = "";

    private TypeAndName() { }

    public static string GetTypeDisplayString(ITypeSymbol symbol)
    {
        StringBuilder builder = new();

        if (symbol is INamedTypeSymbol ints && ints.ContainingNamespace.ToString() == "System")
        {
            return $"global::System.{symbol.Name}";
        }

        var parts = symbol.ToDisplayParts(SymbolDisplayFormat.FullyQualifiedFormat);
        var typeName = String.Join("", parts.TakeWhile(x => x.ToString() != "<"));
        builder.Append(typeName);

        if (symbol is INamedTypeSymbol nts && nts.TypeArguments.Any())
        {
            builder.Append("<");
            bool isFirst = true;
            foreach (var arg in nts.TypeArguments)
            {
                if (!isFirst)
                    builder.Append(", ");
                isFirst = false;
                builder.Append(GetTypeDisplayString(arg));
            }
            builder.Append(">");
        }
        return builder.ToString();
    }

    internal static TypeAndName Parse(IFieldSymbol arg)
    {
        return new TypeAndName
        {
            TypeName = GetTypeDisplayString(arg.Type),
            Name = arg.AssociatedSymbol?.Name ?? arg.Name
        };
    }
}