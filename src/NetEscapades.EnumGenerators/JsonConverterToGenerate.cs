namespace NetEscapades.EnumGenerators;

public readonly record struct JsonConverterToGenerate(
    string ExtensionName,
    string FullyQualifiedName,
    string ExtensionNamespace,
    string ConverterType,
    string? ConverterNamespace,
    bool IsPublic,
    bool CaseSensitive,
    bool CamelCase,
    string? PropertyName);