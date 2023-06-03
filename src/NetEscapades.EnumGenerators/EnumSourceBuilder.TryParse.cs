using System.Text;

namespace NetEscapades.EnumGenerators;

public static partial class EnumSourceBuilder
{
    private static void GenerateTryParseString(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Converts the string representation of the name or numeric value of
                    /// an <see cref="{enumToGenerate.FullyQualifiedName}" /> to the equivalent instance.
                    /// The return value indicates whether the conversion succeeded.
                    /// </summary>
                    /// <param name="name">The case-sensitive string representation of the enumeration name or underlying value to convert.</param>
                    /// <param name="value">When this method returns, contains an object of type
                    /// <see cref="{enumToGenerate.FullyQualifiedName}" /> whose
                    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
                    /// If the parse operation fails, contains the default value of the underlying type
                    /// of <see cref="{enumToGenerate.FullyQualifiedName}" />. This parameter is passed uninitialized.</param>
                    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
                    public static bool TryParse(
                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
                        string? name,
                        out {enumToGenerate.FullyQualifiedName} value)
                        => TryParse(name, out value, false, false);
            """);
    }

    private static void GenerateTryParseStringIgnoreCase(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Converts the string representation of the name or numeric value of
                    /// an <see cref="{enumToGenerate.FullyQualifiedName}" /> to the equivalent instance.
                    /// The return value indicates whether the conversion succeeded.
                    /// </summary>
                    /// <param name="name">The string representation of the enumeration name or underlying value to convert.</param>
                    /// <param name="value">When this method returns, contains an object of type
                    /// <see cref="{enumToGenerate.FullyQualifiedName}" /> whose
                    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
                    /// If the parse operation fails, contains the default value of the underlying type
                    /// of <see cref="{enumToGenerate.FullyQualifiedName}" />. This parameter is passed uninitialized.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
                    public static bool TryParse(
                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
                        string? name,
                        out {enumToGenerate.FullyQualifiedName} value,
                        bool ignoreCase)
                        => TryParse(name, out value, ignoreCase, false);
            """);
    }

    private static void GenerateTryParseStringIgnoreCaseMetadata(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $$"""
                    /// <summary>
                    /// Converts the string representation of the name or numeric value of
                    /// an <see cref="{{enumToGenerate.FullyQualifiedName}}" /> to the equivalent instance.
                    /// The return value indicates whether the conversion succeeded.
                    /// </summary>
                    /// <param name="name">The string representation of the enumeration name or underlying value to convert.</param>
                    /// <param name="value">When this method returns, contains an object of type
                    /// <see cref="{{enumToGenerate.FullyQualifiedName}}" /> whose
                    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
                    /// If the parse operation fails, contains the default value of the underlying type
                    /// of <see cref="{{enumToGenerate.FullyQualifiedName}}" />. This parameter is passed uninitialized.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value included in metadata attributes such as
                    /// <c>[Display]</c> attribute when parsing, otherwise only considers the member names.</param>
                    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
                    public static bool TryParse(
                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
                        string? name,
                        out {{enumToGenerate.FullyQualifiedName}} value,
                        bool ignoreCase,
                        bool allowMatchingMetadataAttribute)
                    {
            """);

        if (enumToGenerate.IsDisplayAttributeUsed)
        {
            sb.AppendLine().Append(
                """
                            if (allowMatchingMetadataAttribute)
                            {
                                if (ignoreCase)
                                {
                                    switch (name)
                                    {
                """);

            foreach (var member in enumToGenerate.Names
                         .Where(static member =>
                             member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence))
            {
                sb.AppendLine().Append(
                    $"""
                                            case not null when name.Equals("{member.Value.DisplayName}", global::System.StringComparison.OrdinalIgnoreCase):
                                                value = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                                return true;
                    """);
            }

            sb.AppendLine().Append(
                """
                                    }
                                }
                                else
                                {
                                    switch (name)
                                    {
                """);

            foreach (var member in enumToGenerate.Names
                         .Where(static member =>
                             member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence))
            {
                sb.AppendLine().Append(
                    $"""
                                            case "{member.Value.DisplayName}":
                                                value = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                                return true;
                    """);
            }

            sb.AppendLine().AppendLine(
                """
                                    }
                                }
                            }
                """);
        }

        sb.AppendLine().Append(
            """
                        if (ignoreCase)
                        {
                            switch (name)
                            {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""
                                    case not null when name.Equals(nameof({enumToGenerate.FullyQualifiedName}.{member.Key}), global::System.StringComparison.OrdinalIgnoreCase):
                                        value = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                        return true;
                """);
        }

        sb.AppendLine().Append(
            $$"""
                                case { Length: > 0 } when {{enumToGenerate.UnderlyingType}}.TryParse(name, out var val):
                                    value = ({{enumToGenerate.FullyQualifiedName}})val;
                                    return true;
                                default:
                                    value = default;
                                    return false;
                            }
                        }
            
                        switch (name)
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""
                                case nameof({enumToGenerate.FullyQualifiedName}.{member.Key}):
                                    value = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                    return true;
                """);
        }

        sb.AppendLine().Append(
            $$"""
                            case { Length: > 0 } when {{enumToGenerate.UnderlyingType}}.TryParse(name, out var val):
                                value = ({{enumToGenerate.FullyQualifiedName}})val;
                                return true;
                            default:
                                value = default;
                                return false;
                        }
                    }
            """);
    }

    private static void GenerateTryParseSpan(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Converts the span representation of the name or numeric value of
                    /// an <see cref="{enumToGenerate.FullyQualifiedName}" /> to the equivalent instance.
                    /// The return value indicates whether the conversion succeeded.
                    /// </summary>
                    /// <param name="name">The span representation of the enumeration name or underlying value to convert.</param>
                    /// <param name="value">When this method returns, contains an object of type
                    /// <see cref="{enumToGenerate.FullyQualifiedName}" /> whose
                    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
                    /// If the parse operation fails, contains the default value of the underlying type
                    /// of <see cref="{enumToGenerate.FullyQualifiedName}" />. This parameter is passed uninitialized.</param>
                    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
                    public static bool TryParse(
                        in global::System.ReadOnlySpan<char> name,
                        out {enumToGenerate.FullyQualifiedName} value)
                        => TryParse(name, out value, false, false);
            """);
    }

    private static void GenerateTryParseSpanIgnoreCase(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $"""
                    /// <summary>
                    /// Converts the span representation of the name or numeric value of
                    /// an <see cref="{enumToGenerate.FullyQualifiedName}" /> to the equivalent instance.
                    /// The return value indicates whether the conversion succeeded.
                    /// </summary>
                    /// <param name="name">The span representation of the enumeration name or underlying value to convert.</param>
                    /// <param name="value">When this method returns, contains an object of type
                    /// <see cref="{enumToGenerate.FullyQualifiedName}" /> whose
                    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
                    /// If the parse operation fails, contains the default value of the underlying type
                    /// of <see cref="{enumToGenerate.FullyQualifiedName}" />. This parameter is passed uninitialized.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
                    public static bool TryParse(
                        in global::System.ReadOnlySpan<char> name,
                        out {enumToGenerate.FullyQualifiedName} value,
                        bool ignoreCase)
                        => TryParse(name, out value, ignoreCase, false);
            """);
    }

    private static void GenerateTryParseSpanIgnoreCaseMetadata(StringBuilder sb, in EnumToGenerate enumToGenerate)
    {
        sb.AppendLine().AppendLine().Append(
            $$"""
                    /// <summary>
                    /// Converts the span representation of the name or numeric value of
                    /// an <see cref="{{enumToGenerate.FullyQualifiedName}}" /> to the equivalent instance.
                    /// The return value indicates whether the conversion succeeded.
                    /// </summary>
                    /// <param name="name">The span representation of the enumeration name or underlying value to convert.</param>
                    /// <param name="result">When this method returns, contains an object of type
                    /// <see cref="{{enumToGenerate.FullyQualifiedName}}" /> whose
                    /// value is represented by <paramref name="result"/> if the parse operation succeeds.
                    /// If the parse operation fails, contains the default value of the underlying type
                    /// of <see cref="{{enumToGenerate.FullyQualifiedName}}" />. This parameter is passed uninitialized.</param>
                    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
                    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value included in metadata attributes such as
                    /// <c>[Display]</c> attribute when parsing, otherwise only considers the member names.</param>
                    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
                    public static bool TryParse(
                        in global::System.ReadOnlySpan<char> name,
                        out {{enumToGenerate.FullyQualifiedName}} result,
                        bool ignoreCase,
                        bool allowMatchingMetadataAttribute)
                    {
            """);

        if (enumToGenerate.IsDisplayAttributeUsed)
        {
            sb.AppendLine().Append(
                """
                            if (allowMatchingMetadataAttribute)
                            {
                                if (ignoreCase)
                                {
                                    switch (name)
                                    {
                """);

            foreach (var member in enumToGenerate.Names
                         .Where(static member =>
                             member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence))
            {
                sb.AppendLine().Append(
                    $"""
                                            case var current when global::System.MemoryExtensions.Equals(current, {member.Key.GetPrivateMetadataMemoryFieldName()}.Span, global::System.StringComparison.OrdinalIgnoreCase):
                                                result = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                                return true;
                    """);
            }

            sb.AppendLine().Append(
                """
                                    }
                                }
                                else
                                {
                                    switch (name)
                                    {
                """);

            foreach (var member in enumToGenerate.Names
                         .Where(static member =>
                             member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence))
            {
                sb.AppendLine().Append(
                    $"""
                                            case var current when global::System.MemoryExtensions.Equals(current, {member.Key.GetPrivateMetadataMemoryFieldName()}.Span, global::System.StringComparison.Ordinal):
                                                result = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                                return true;
                    """);
            }

            sb.AppendLine().Append(
                """
                                    }
                                }
                            }
                """).AppendLine();
        }

        sb.AppendLine().Append(
            """
                        if (ignoreCase)
                        {
                            switch (name)
                            {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""
                                    case var current when global::System.MemoryExtensions.Equals(current, {member.Key.GetPrivateMemoryFieldName()}.Span, global::System.StringComparison.OrdinalIgnoreCase):
                                        result = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                        return true;
                """);
        }

        sb.AppendLine().Append(
            $$"""
                                case { IsEmpty: false } when {{enumToGenerate.UnderlyingType}}.TryParse(name, out var numericResult):
                                    result = ({{enumToGenerate.FullyQualifiedName}})numericResult;
                                    return true;
                                default:
                                    result = default;
                                    return false;
                            }
                        }

                        switch (name)
                        {
            """);

        foreach (var member in enumToGenerate.Names)
        {
            sb.AppendLine().Append(
                $"""
                                case var current when global::System.MemoryExtensions.Equals(current, {member.Key.GetPrivateMemoryFieldName()}.Span, global::System.StringComparison.Ordinal):
                                    result = {enumToGenerate.FullyQualifiedName}.{member.Key};
                                    return true;
                """);
        }

        sb.AppendLine().Append(
            $$"""
                            case { IsEmpty: false } when {{enumToGenerate.UnderlyingType}}.TryParse(name, out var numericResult):
                                result = ({{enumToGenerate.FullyQualifiedName}})numericResult;
                                return true;
                            default:
                                result = default;
                                return false;
                        }
                    }
            """);
    }
}
