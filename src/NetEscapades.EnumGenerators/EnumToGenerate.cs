namespace NetEscapades.EnumGenerators;

/// <summary>
/// Key is the enum name.
/// </summary>
public readonly record struct EnumToGenerate(
    string Name,
    string FullyQualifiedName,
    string Namespace,
    bool IsPublic,
    bool HasFlags,
    string UnderlyingType,
    bool IsDisplayAttributeUsed,
    EquatableArray<(string Key, EnumValueOption Value)> Names);
