using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NetEscapades.EnumGenerators;

[Generator]
public class EnumGenerator : IIncrementalGenerator
{
    private const string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";
    private const string DescriptionAttribute = "System.ComponentModel.DescriptionAttribute";
    private const string EnumExtensionsAttribute = "NetEscapades.EnumGenerators.EnumExtensionsAttribute";
    private const string FlagsAttribute = "System.FlagsAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx => ctx.AddSource(
            "EnumExtensionsAttribute.g.cs", SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        var enumsToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                EnumExtensionsAttribute,
                static (node, _) => node is EnumDeclarationSyntax,
                GetTypeToGenerate)
            .Where(static m => m is not null);

        context.RegisterSourceOutput(enumsToGenerate,
            static (spc, enumToGenerate) => Execute(in enumToGenerate, spc));
    }

    private static void Execute(in EnumToGenerate? enumToGenerate, SourceProductionContext context)
    {
        if (enumToGenerate is { } eg)
        {
            StringBuilder sb = new();
            var result = SourceGenerationHelper.GenerateExtensionClass(sb, in eg);
            context.AddSource(eg.Name + "_EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private static EnumToGenerate? GetTypeToGenerate(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        if (context.TargetSymbol is not INamedTypeSymbol enumSymbol)
        {
            // nothing to do if this type isn't available
            return null;
        }

        ct.ThrowIfCancellationRequested();

        var name = enumSymbol.Name + "Extensions";
        var nameSpace = enumSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : enumSymbol.ContainingNamespace.ToString();
        var hasFlags = false;

        foreach (var attributeData in enumSymbol.GetAttributes())
        {
            if (attributeData.AttributeClass?.Name is "FlagsAttribute" or "Flags" &&
                attributeData.AttributeClass.ToDisplayString() == FlagsAttribute)
            {
                hasFlags = true;
                continue;
            }

            if (attributeData.AttributeClass?.Name != "EnumExtensionsAttribute" ||
                attributeData.AttributeClass.ToDisplayString() != EnumExtensionsAttribute)
            {
                continue;
            }

            foreach (var namedArgument in attributeData.NamedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "ExtensionClassNamespace" when namedArgument.Value.Value?.ToString() is { } ns:
                        nameSpace = ns;
                        continue;
                    case "ExtensionClassName" when namedArgument.Value.Value?.ToString() is { } n:
                        name = n;
                        break;
                }
            }
        }

        var fullyQualifiedName = enumSymbol.ToString();
        var underlyingType = enumSymbol.EnumUnderlyingType?.ToString() ?? "int";

        var enumMembers = enumSymbol.GetMembers();
        var members = new List<(string, EnumValueOption)>(enumMembers.Length);
        HashSet<string>? displayNames = null;
        var isDisplayNameTheFirstPresence = false;

        foreach (var member in enumMembers)
        {
            if (member is not IFieldSymbol field
                || field.ConstantValue is null)
            {
                continue;
            }

            string? displayName = null;
            foreach (var attribute in member.GetAttributes())
            {
                if (attribute.AttributeClass?.Name == "DisplayAttribute" &&
                    attribute.AttributeClass.ToDisplayString() == DisplayAttribute)
                {
                    foreach (var namedArgument in attribute.NamedArguments)
                    {
                        if (namedArgument.Key == "Name" && namedArgument.Value.Value?.ToString() is { } dn)
                        {
                            // found display attribute, all done
                            displayName = dn;
                            goto addDisplayName;
                        }
                    }
                }

                if (attribute.AttributeClass?.Name == "DescriptionAttribute"
                    && attribute.AttributeClass.ToDisplayString() == DescriptionAttribute
                    && attribute.ConstructorArguments.Length == 1)
                {
                    if (attribute.ConstructorArguments[0].Value?.ToString() is { } dn)
                    {
                        // found display attribute, all done
                        displayName = dn;
                        goto addDisplayName;
                    }
                }
            }

            addDisplayName:
            if (displayName is not null)
            {
                displayNames ??= new();
                isDisplayNameTheFirstPresence = displayNames.Add(displayName);
            }

            members.Add((member.Name, new EnumValueOption(displayName, isDisplayNameTheFirstPresence)));
        }

        return new EnumToGenerate(
            name,
            fullyQualifiedName,
            nameSpace,
            enumSymbol.DeclaredAccessibility == Accessibility.Public,
            hasFlags,
            underlyingType,
            displayNames?.Count > 0,
            new EquatableArray<(string Key, EnumValueOption Value)>(members.ToArray())
        );
    }
}
