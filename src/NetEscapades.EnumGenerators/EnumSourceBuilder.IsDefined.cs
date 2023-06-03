using System.Text;

namespace NetEscapades.EnumGenerators;

public static partial class EnumSourceBuilder
{
    private static void GenerateIsDefinedEnum(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $$"""
                    /// <summary>
                    /// Returns a boolean telling whether the given enum value exists in the enumeration.
                    /// </summary>
                    /// <param name="value">The value to check if it's defined.</param>
                    /// <returns><see langword="true" /> if the value exists in the enumeration, <see langword="false" /> otherwise.</returns>
                    public static bool IsDefined({{enumToGenerate.FullyQualifiedName}} value)
                        => value switch
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine()
                .Append(
                    $"""                {enumToGenerate.FullyQualifiedName}.{member.Key} => true,""");
        }

        sb.AppendLine().Append(
            """
                            _ => false
                        };
            """);
    }

    private static void GenerateIsDefinedString(StringBuilder sb)
    {
        sb.AppendLine().AppendLine().Append(
            """
                    /// <summary>
                    /// Returns a boolean telling whether an enum with the given name exists in the enumeration.
                    /// </summary>
                    /// <param name="name">The name to check if it's defined.</param>
                    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, <see langword="false" /> otherwise.</returns>
                    public static bool IsDefined(string name) => IsDefined(name, false);
            """);
    }

    private static void GenerateIsDefinedStringMetadata(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            """
                    /// <summary>
                    /// Returns a boolean telling whether an enum with the given name exists in the enumeration,
                    /// or if a member decorated with a <c>[Display]</c> attribute
                    /// with the required name exists.
                    /// </summary>
                    /// <param name="name">The name to check if it's defined.</param>
                    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value of metadata attributes, otherwise ignores them.</param>
                    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, or a member is decorated
                    /// with a <c>[Display]</c> attribute with the name, <see langword="false" /> otherwise.</returns>
                    public static bool IsDefined(string name, bool allowMatchingMetadataAttribute)
                    {
            """);

        if (enumToGenerate.IsDisplayAttributeUsed)
        {
            sb.AppendLine().Append(
                """
                                var isDefinedInDisplayAttribute = false;
                                if (allowMatchingMetadataAttribute)
                                {
                                    isDefinedInDisplayAttribute = name switch
                                    {
                    """);

            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence)
                {
                    sb.AppendLine().Append($"""                    "{member.Value.DisplayName}" => true,""");
                }
            }

            sb.AppendLine()
                .Append(
                    """
                                        _ => false
                                    };
                                }

                                if (isDefinedInDisplayAttribute)
                                {
                                    return true;
                                }
                    """).AppendLine();
        }

        sb.AppendLine()
            .Append(
                """
                            return name switch
                            {
                """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""                nameof({enumToGenerate.FullyQualifiedName}.{member.Key}) => true,""");
        }

        sb.AppendLine().Append(
            """
                            _ => false
                        };
                    }
            """);
    }

    private static void GenerateIsDefinedSpan(StringBuilder sb)
    {
        sb.AppendLine()
            .AppendLine()
            .Append(
                """
                /// <summary>
                /// Returns a boolean telling whether an enum with the given name exists in the enumeration.
                /// </summary>
                /// <param name="name">The name to check if it's defined.</param>
                /// <returns><see langword="true" /> if a member with the name exists in the enumeration, <see langword="false" /> otherwise.</returns>
                public static bool IsDefined(in global::System.ReadOnlySpan<char> name) => IsDefined(name, false);
        """);
    }

    private static void GenerateIsDefinedSpanMetadata(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            """
                    /// <summary>
                    /// Returns a boolean telling whether an enum with the given name exists in the enumeration,
                    /// or optionally if a member decorated with a <c>[Display]</c> attribute
                    /// with the required name exists.
                    /// </summary>
                    /// <param name="name">The name to check if it's defined.</param>
                    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value of metadata attributes, otherwise ignores them.</param>
                    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, or a member is decorated
                    /// with a <c>[Display]</c> attribute with the name, <see langword="false" /> otherwise.</returns>
                    public static bool IsDefined(in global::System.ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
                    {
            """);

        if (enumToGenerate.IsDisplayAttributeUsed)
        {
            sb.AppendLine().Append(
                """
                            var isDefinedInDisplayAttribute = false;
                            if (allowMatchingMetadataAttribute)
                            {
                                isDefinedInDisplayAttribute = name switch
                                {
                """);

            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence)
                {
                    sb.AppendLine()
                        .Append(
                            $"""                    var current when global::System.MemoryExtensions.Equals(current, {member.Key.GetPrivateMetadataMemoryFieldName()}.Span, global::System.StringComparison.Ordinal) => true,""");
                }
            }

            sb.AppendLine().Append(
                """
                                    _ => false
                                };
                            }

                            if (isDefinedInDisplayAttribute)
                            {
                                return true;
                            }
                """).AppendLine();
        }

        sb.AppendLine().Append(
            """
                        return name switch
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""                var current when global::System.MemoryExtensions.Equals(current, {member.Key.GetPrivateMemoryFieldName()}.Span, global::System.StringComparison.Ordinal) => true,""");
        }

        sb.AppendLine().Append(
            """
                            _ => false
                        };
                    }
            """);
    }
}
