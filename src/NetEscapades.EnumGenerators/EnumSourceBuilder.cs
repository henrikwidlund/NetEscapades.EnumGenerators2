using System.Text;

namespace NetEscapades.EnumGenerators;

public static partial class EnumSourceBuilder
{
    internal const string Attribute = $$"""
#if NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES

{{Constants.GeneratedCodeHeader}}

namespace NetEscapades.EnumGenerators
{
    /// <summary>
    /// Add to enums to indicate that extension methods should be generated for the type.
    /// </summary>
    [global::System.AttributeUsage(global::System.AttributeTargets.Enum)]
    [global::System.Diagnostics.Conditional(""NETESCAPADES_ENUMGENERATORS_USAGES"")]
    public class EnumExtensionsAttribute : global::System.Attribute
    {
        /// <summary>
        /// The namespace to generate the extension class.
        /// If not provided the namespace of the enum will be used.
        /// </summary>
        public string? ExtensionClassNamespace { get; set; }

        /// <summary>
        /// The name to use for the extension class.
        /// If not provided, the enum name with ""Extensions"" will be used.
        /// For example for an Enum called StatusCodes, the default name
        /// will be StatusCodesExtensions.
        /// </summary>
        public string? ExtensionClassName { get; set; }
    }
}
#endif
""";

    public static string GenerateExtensionClass(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        GenerateEnumExtensionClassDeclaration(sb, enumToGenerate);
        GenerateEnumLengthConst(sb, enumToGenerate);
        GenerateToStringFast(sb, enumToGenerate);
        GenerateHasFlagFast(sb, enumToGenerate);
        GenerateIsDefined(sb, enumToGenerate);
        GenerateMemoryBackingFields(sb, enumToGenerate);
        GenerateTryParse(sb, enumToGenerate);
        GenerateGetMetadataNamesOrDefault(sb, enumToGenerate);
        GenerateGetValueOrDefault(sb, enumToGenerate);
        GenerateGetValues(sb, enumToGenerate);
        GenerateGetNames(sb, enumToGenerate);
        GenerateEnumExtensionClassEnd(sb, enumToGenerate);

        return sb.ToString();
    }

    private static void GenerateEnumExtensionClassDeclaration(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb
            .Append(Constants.GeneratedCodeHeader)
            .AppendLine()
            .AppendLine()
            .AppendLine("""
            using System;
            """);

        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb
                .AppendLine()
                .Append($$"""
                    namespace {{enumToGenerate.Namespace}}
                    {
                    """);
        }

        sb.AppendLine().Append(
                $$"""
                    /// <summary>
                    /// Extension methods for <see cref="{{enumToGenerate.FullyQualifiedName}}" />.
                    /// </summary>
                    {{(enumToGenerate.IsPublic ? "public" : "internal")}} static partial class {{enumToGenerate.Name}}
                    {
                """);
    }

    private static void GenerateEnumLengthConst(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.Append(
            $"""

                    /// <summary>
                    /// The number of members in the enum.
                    /// This is a non-distinct count of defined names.
                    /// </summary>
                    public const int Length = {enumToGenerate.Names.Count.ToString()};
            """);
    }

    private static void GenerateToStringFast(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().Append(
            $$"""
            
                    /// <summary>
                    /// Returns the string representation of the <see cref="{{enumToGenerate.FullyQualifiedName}}"/> value.
                    /// If the attribute is decorated with a <c>[Display]</c> attribute, then
                    /// uses the provided value. Otherwise uses the name of the member, equivalent to
                    /// calling <c>ToString()</c> on <paramref name="value"/>.
                    /// </summary>
                    /// <param name="value">The value to retrieve the string value for.</param>
                    /// <returns>The string representation of the value.</returns>
                    public static string ToStringFast(this {{enumToGenerate.FullyQualifiedName}} value)
                        => value switch
                        {
            """);
        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""                {enumToGenerate.FullyQualifiedName}.{member.Key} => """);

            sb.Append(member.Value.DisplayName is null
                ? $"nameof({enumToGenerate.FullyQualifiedName}.{member.Key}),"
                : $"""
                    "{member.Value.DisplayName}",
                    """);
        }

        sb.AppendLine().Append(
            """
                            _ => value.ToString()
                        };
            """);
    }

    private static void GenerateHasFlagFast(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        if (enumToGenerate.HasFlags)
        {
            sb
                .AppendLine()
                .AppendLine()
                .Append(
                $"""
                        /// <summary>
                        /// Determines whether one or more bit fields are set in the current instance.
                        /// Equivalent to calling <see cref="global::System.Enum.HasFlag" /> on <paramref name="value"/>.
                        /// </summary>
                        /// <param name="value">The value of the instance to investigate.</param>
                        /// <param name="flag">The flag to check for.</param>
                        /// <returns><see langword="true" /> if the fields set in the flag are also set in the current instance; otherwise <see langword="false" />.</returns>
                        /// <remarks>If the underlying value of <paramref name="flag"/> is zero, the method returns <see langword="true" />.
                        /// This is consistent with the behaviour of <see cref="global::System.Enum.HasFlag" />.</remarks>
                        public static bool HasFlagFast(this {enumToGenerate.FullyQualifiedName} value, {enumToGenerate.FullyQualifiedName} flag)
                            => flag == 0 || (value & flag) == flag;
                """);
        }
    }

    private static void GenerateIsDefined(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        GenerateIsDefinedEnum(sb, enumToGenerate);
        GenerateIsDefinedString(sb);
        GenerateIsDefinedStringMetadata(sb, enumToGenerate);
        GenerateIsDefinedSpan(sb);
        GenerateIsDefinedSpanMetadata(sb, enumToGenerate);
    }

    private static void GenerateMemoryBackingFields(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine();
        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine()
                .Append(
                    $"""        private static readonly ReadOnlyMemory<char> {member.Key.GetPrivateMemoryFieldName()} = "{member.Key}".AsMemory();""");
        }

        foreach (var member in enumToGenerate.Names.Where(static m =>
                     m.Value.DisplayName is not null && m.Value.IsDisplayNameTheFirstPresence))
        {
            sb.AppendLine()
                .Append(
                    $"""        private static readonly ReadOnlyMemory<char> {member.Key.GetPrivateMetadataMemoryFieldName()} = "{member.Value.DisplayName}".AsMemory();""");
        }
    }

    private static void GenerateTryParse(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        GenerateTryParseString(sb, enumToGenerate);
        GenerateTryParseStringIgnoreCase(sb, enumToGenerate);
        GenerateTryParseStringIgnoreCaseMetadata(sb, enumToGenerate);
        GenerateTryParseSpan(sb, enumToGenerate);
        GenerateTryParseSpanIgnoreCase(sb, enumToGenerate);
        GenerateTryParseSpanIgnoreCaseMetadata(sb, enumToGenerate);
    }

    private static void GenerateGetMetadataNamesOrDefault(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $$"""
                    /// <summary>
                    /// Retrieves an array of the metadata or <see langword="default" /> values of the members defined in
                    /// <see cref="{{enumToGenerate.FullyQualifiedName}}" />.
                    /// Note that this returns a new array with every invocation, so
                    /// should be cached if appropriate.
                    /// </summary>
                    /// <returns>An array of the metadata or <see langword="default" /> values defined in <see cref="{{enumToGenerate.FullyQualifiedName}}" />.</returns>
                    public static string[] GetMetadataNamesOrDefault() =>
                        new[]
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                member.Value.DisplayName is not null
                    ? $"""                "{member.Value.DisplayName}","""
                    : $"""                nameof({enumToGenerate.FullyQualifiedName}.{member.Key}),""");
        }

        sb.AppendLine().Append(
            """
                        };
            """);
    }

    private static void GenerateGetValueOrDefault(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Gets the <see cref="{enumToGenerate.FullyQualifiedName}" /> representation of the <paramref name="name"/>
                    /// or <see langword="default" /> if there's no match.
                    /// </summary>
                    /// <param name="name">The value that should be matched.</param>
                    /// <returns>The matching <see cref="{enumToGenerate.FullyQualifiedName}" /> or <see langword="null" /> if there was no match.</returns>
                    public static {enumToGenerate.FullyQualifiedName}? GetValueOrDefault(string? name) =>
                        TryParse(name, out {enumToGenerate.FullyQualifiedName} value) ? value : null;
            """);

        sb.AppendLine().AppendLine().Append(
                $"""
                        /// <summary>
                        /// Gets the <see cref="{enumToGenerate.FullyQualifiedName}" /> representation of the <paramref name="name"/>
                        /// or <see langword="default" /> if there's no match.
                        /// </summary>
                        /// <param name="name">The value that should be matched.</param>
                        /// <returns>The matching <see cref="{enumToGenerate.FullyQualifiedName}" /> or <see langword="null" /> if there was no match.</returns>
                        public static {enumToGenerate.FullyQualifiedName}? GetValueOrDefault(in ReadOnlySpan<char> name) =>
                            TryParse(name, out {enumToGenerate.FullyQualifiedName} value) ? value : null;
                """);

        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Gets the <see cref="{enumToGenerate.FullyQualifiedName}" /> representation of the <paramref name="name"/>
                    /// or <see langword="default" /> if there's no match.
                    /// </summary>
                    /// <param name="name">The value that should be matched.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
                    /// <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <returns>The matching <see cref="{enumToGenerate.FullyQualifiedName}" /> or <see langword="null" /> if there was no match.</returns>
                    public static {enumToGenerate.FullyQualifiedName}? GetValueOrDefault(string? name, bool ignoreCase) =>
                        TryParse(name, out {enumToGenerate.FullyQualifiedName} value, ignoreCase) ? value : null;
            """);

        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Gets the <see cref="{enumToGenerate.FullyQualifiedName}" /> representation of the <paramref name="name"/>
                    /// or <see langword="default" /> if there's no match.
                    /// </summary>
                    /// <param name="name">The value that should be matched.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
                    /// <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <returns>The matching <see cref="{enumToGenerate.FullyQualifiedName}" /> or <see langword="null" /> if there was no match.</returns>
                    public static {enumToGenerate.FullyQualifiedName}? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase) =>
                        TryParse(name, out {enumToGenerate.FullyQualifiedName} value, ignoreCase) ? value : null;
            """);

        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Gets the <see cref="{enumToGenerate.FullyQualifiedName}" /> representation of the <paramref name="name"/>
                    /// or <see langword="default" /> if there's no match.
                    /// </summary>
                    /// <param name="name">The value that should be matched.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
                    /// <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />,
                    /// considers the value of metadata attributes, otherwise ignores them.</param>
                    /// <returns>The matching <see cref="{enumToGenerate.FullyQualifiedName}" /> or <see langword="null" /> if there was no match.</returns>
                    public static {enumToGenerate.FullyQualifiedName}? GetValueOrDefault(string? name, bool ignoreCase, bool allowMatchingMetadataAttribute) =>
                        TryParse(name, out {enumToGenerate.FullyQualifiedName} value, ignoreCase, allowMatchingMetadataAttribute) ? value : null;
            """);

        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Gets the <see cref="{enumToGenerate.FullyQualifiedName}" /> representation of the <paramref name="name"/>
                    /// or <see langword="default" /> if there's no match.
                    /// </summary>
                    /// <param name="name">The value that should be matched.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
                    /// <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />,
                    /// considers the value of metadata attributes, otherwise ignores them.</param>
                    /// <returns>The matching <see cref="{enumToGenerate.FullyQualifiedName}" /> or <see langword="null" /> if there was no match.</returns>
                    public static {enumToGenerate.FullyQualifiedName}? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute) =>
                        TryParse(name, out {enumToGenerate.FullyQualifiedName} value, ignoreCase, allowMatchingMetadataAttribute) ? value : null;
            """);
    }

    private static void GenerateGetValues(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $$"""
                    /// <summary>
                    /// Retrieves an array of the values of the members defined in
                    /// <see cref="{{enumToGenerate.FullyQualifiedName}}" />.
                    /// Note that this returns a new array with every invocation, so
                    /// should be cached if appropriate.
                    /// </summary>
                    /// <returns>An array of the values defined in <see cref="{{enumToGenerate.FullyQualifiedName}}" />.</returns>
                    public static {{enumToGenerate.FullyQualifiedName}}[] GetValues() =>
                        new[]
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""                {enumToGenerate.FullyQualifiedName}.{member.Key},""");
        }

        sb.AppendLine().Append(
            """
                        };
            """);
    }

    private static void GenerateGetNames(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $$"""
                    /// <summary>
                    /// Retrieves an array of the names of the members defined in
                    /// <see cref="{{enumToGenerate.FullyQualifiedName}}" />.
                    /// Note that this returns a new array with every invocation, so
                    /// should be cached if appropriate.
                    /// </summary>
                    /// <returns>An array of the names of the members defined in <see cref="{{enumToGenerate.FullyQualifiedName}}" />.</returns>
                    public static string[] GetNames() =>
                        new[]
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(
                $"""
                
                                nameof({enumToGenerate.FullyQualifiedName}.{member.Key}),
                """);
        }

        sb.AppendLine().Append("            };");
    }

    private static void GenerateEnumExtensionClassEnd(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().Append(
            """
                }
            """);

        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb.AppendLine().Append(
                """
                }
                """);
        }

        sb.AppendLine();
    }
}