namespace NetEscapades.EnumGenerators;

/// <param name="DisplayName">Custom name set by the <c>[Display(Name)]</c> attribute.</param>
public readonly record struct EnumValueOption(
    string? DisplayName,
    bool IsDisplayNameTheFirstPresence
);
