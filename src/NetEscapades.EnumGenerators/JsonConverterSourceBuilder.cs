using System.Text;

namespace NetEscapades.EnumGenerators;

public static class JsonConverterSourceBuilder
{
    internal const string Attribute = $$"""
#if NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES

{{Constants.GeneratedCodeHeader}}

namespace NetEscapades.EnumGenerators
{
    /// <summary>
    /// Add to enums to indicate that a JsonConverter for the enum should be generated.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    [System.Diagnostics.Conditional(""NETESCAPADES_ENUMGENERATORS_USAGES"")]
    public class EnumJsonConverterAttribute : System.Attribute
    {
        /// <summary>
        /// The converter that should be generated.
        /// </summary>
        public System.Type ConverterType { get; }

        /// <summary>
        /// Indicates if the string representation is case sensitive when deserializing it as an enum.
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Indicates if the value of <see cref=""PropertyName""/> should be camel cased.
        /// </summary>
        public bool CamelCase { get; set; }

        /// <summary>
        /// If set, this value will be used in messages when there are problems with validation and/or serialization/deserialization occurs.
        /// </summary>
        public string? PropertyName { get; set; }
        
        /// <summary>
        /// Creates an instance of <see cref=""EnumJsonConverterAttribute""/>.
        /// </summary>
        /// <param name=""converterType"">The converter to generate.</param>
        public EnumJsonConverterAttribute(System.Type converterType) => ConverterType = converterType;
    }
}
#endif
""";

    public static string GenerateJsonConverterClass(StringBuilder sb, JsonConverterToGenerate jsonConverterToGenerate)
    {
        sb.Append(Constants.GeneratedCodeHeader)
            .AppendLine();

        if (!string.IsNullOrEmpty(jsonConverterToGenerate.ConverterNamespace))
        {
            sb
                .Append("namespace ")
                .Append(jsonConverterToGenerate.ConverterNamespace)
                .Append(";")
                .AppendLine();
        }

        var fullyQualifiedExtension = string.IsNullOrEmpty(jsonConverterToGenerate.ExtensionNamespace)
            ? jsonConverterToGenerate.ExtensionName
            : $"{jsonConverterToGenerate.ExtensionNamespace}.{jsonConverterToGenerate.ExtensionName}";

        sb.AppendLine()
            .AppendLine($"""
            /// <summary>
            /// Converts a <see cref="{jsonConverterToGenerate.FullyQualifiedName}" /> to or from JSON.
            /// </summary>
            """)
            .Append(jsonConverterToGenerate.IsPublic ? "public" : "internal").Append(" sealed class ")
            .Append(jsonConverterToGenerate.ConverterType)
            .Append(" : global::System.Text.Json.Serialization.JsonConverter<")
            .Append(jsonConverterToGenerate.FullyQualifiedName)
            .AppendLine(">")
            .AppendLine("{");

        var propertyName = jsonConverterToGenerate.PropertyName;
        if (!string.IsNullOrEmpty(propertyName) && jsonConverterToGenerate.CamelCase)
        {
            propertyName = propertyName.ToCamelCase();
        }

        if (!string.IsNullOrEmpty(propertyName))
            sb.Append("     private const string PropertyName = \"")
                .Append(propertyName)
                .AppendLine("\";")
                .AppendLine();

        sb.AppendLine($"""
                 /// <inheritdoc />
                 /// <summary>
                 /// Read and convert the JSON to <see cref="{jsonConverterToGenerate.FullyQualifiedName}" />.
                 /// </summary>
                 /// <remarks>
                 /// A converter may throw any Exception, but should throw <see cref="global::System.Text.Json.JsonException" /> when the JSON is invalid.
                 /// </remarks>
            """)
            .Append("     public override ")
            .Append(jsonConverterToGenerate.FullyQualifiedName)
            .AppendLine(" Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)")
            .AppendLine("     {")
            .AppendLine("         var value = reader.GetString();")
            .Append("         if (")
            .Append(fullyQualifiedExtension)
            .Append(".TryParse(value, out var enumValue, ")
            .Append(jsonConverterToGenerate.CaseSensitive ? "false" : "true")
            .AppendLine(", true))")
            .AppendLine("            return enumValue;")
            .AppendLine()
            .Append("         throw new global::System.Text.Json.JsonException($\"{value} is not a valid value.\", ")
            .Append(string.IsNullOrEmpty(propertyName) ? "null" : "PropertyName")
            .AppendLine(", null, null);")
            .AppendLine("     }")
            .AppendLine()
            .AppendLine("""
                 /// <inheritdoc />
            """)
            .Append("     public override void Write(global::System.Text.Json.Utf8JsonWriter writer, ")
            .Append(jsonConverterToGenerate.FullyQualifiedName)
            .AppendLine(" value, global::System.Text.Json.JsonSerializerOptions options)")
            .Append("         => writer.WriteStringValue(").Append(fullyQualifiedExtension).AppendLine(".ToStringFast(value));")
            .AppendLine("}");

        return sb.ToString();
    }
}