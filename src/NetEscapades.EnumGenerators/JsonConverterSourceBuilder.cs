using System.Text;

namespace NetEscapades.EnumGenerators;

public static class JsonConverterSourceBuilder
{
    internal const string Attribute = $$"""
#if NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES

{{Constants.GeneratedCodeHeader}}

[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Generated by the NetEscapades.EnumGenerators source generator.")]
namespace NetEscapades.EnumGenerators
{
    /// <summary>
    /// Add to enums to indicate that a JsonConverter for the enum should be generated.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    [System.Diagnostics.Conditional("NETESCAPADES_ENUMGENERATORS_USAGES")]
    public class EnumJsonConverterAttribute : System.Attribute
    {
        /// <summary>
        /// The converter that should be generated.
        /// </summary>
        public global::System.Type ConverterType { get; }

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
        /// Creates an instance of <see cref="EnumJsonConverterAttribute"/>.
        /// </summary>
        /// <param name="converterType">The converter to generate.</param>
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
            sb.AppendLine($"namespace {jsonConverterToGenerate.ConverterNamespace};");

        sb.AppendLine()
            .AppendLine($$"""
            /// <summary>
            /// Converts a <see cref="global::{{jsonConverterToGenerate.FullyQualifiedName}}" /> to or from JSON.
            /// </summary>
            {{(jsonConverterToGenerate.IsPublic ? "public" : "internal")}} sealed class {{jsonConverterToGenerate.ConverterType}} : global::System.Text.Json.Serialization.JsonConverter<{{jsonConverterToGenerate.FullyQualifiedName}}>
            {
            """);

        var propertyName = jsonConverterToGenerate.PropertyName;
        if (!string.IsNullOrEmpty(propertyName) && jsonConverterToGenerate.CamelCase)
            propertyName = propertyName.ToCamelCase();

        if (!string.IsNullOrEmpty(propertyName))
            sb.AppendLine($"""    private const string PropertyName = "{propertyName}";""")
                .AppendLine();

        var fullyQualifiedExtension = string.IsNullOrEmpty(jsonConverterToGenerate.ExtensionNamespace)
            ? jsonConverterToGenerate.ExtensionName
            : $"{jsonConverterToGenerate.ExtensionNamespace}.{jsonConverterToGenerate.ExtensionName}";

        sb.AppendLine($$"""
                /// <inheritdoc />
                /// <summary>
                /// Read and convert the JSON to <see cref="global::{{jsonConverterToGenerate.FullyQualifiedName}}" />.
                /// </summary>
                /// <remarks>
                /// A converter may throw any Exception, but should throw <see cref="global::System.Text.Json.JsonException" /> when the JSON is invalid.
                /// </remarks>
                public override global::{{jsonConverterToGenerate.FullyQualifiedName}} Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                {
                    char[]? rentedBuffer = null;
                    var bufferLength = reader.HasValueSequence ? checked((int)reader.ValueSequence.Length) : reader.ValueSpan.Length;

                    var charBuffer = bufferLength <= 128
                        ? stackalloc char[128]
                        : rentedBuffer = global::System.Buffers.ArrayPool<char>.Shared.Rent(bufferLength);

                    var charsWritten = reader.CopyString(charBuffer);
                    global::System.ReadOnlySpan<char> source = charBuffer[..charsWritten];
                    try
                    {
                        if (global::{{fullyQualifiedExtension}}.TryParse(source, out var enumValue, {{(jsonConverterToGenerate.CaseSensitive ? "false" : "true")}}, {{(jsonConverterToGenerate.AllowMatchingMetadataAttribute ? "true))" : "false))")}}
                            return enumValue;

                        throw new global::System.Text.Json.JsonException($"{source.ToString()} is not a valid value.", {{(string.IsNullOrEmpty(propertyName) ? "null" : "PropertyName")}}, null, null);
                    }
                    finally
                    {
                        if (rentedBuffer is not null)
                        {
                            charBuffer[..charsWritten].Clear();
                            global::System.Buffers.ArrayPool<char>.Shared.Return(rentedBuffer);
                        }
                    }
                }
            """)
            .AppendLine()
            .AppendLine($$"""
                /// <inheritdoc />
                public override void Write(global::System.Text.Json.Utf8JsonWriter writer, global::{{jsonConverterToGenerate.FullyQualifiedName}} value, global::System.Text.Json.JsonSerializerOptions options)
                    => writer.WriteStringValue(global::{{fullyQualifiedExtension}}.ToStringFast(value));
            """)
            .AppendLine("}");

        return sb.ToString();
    }
}
