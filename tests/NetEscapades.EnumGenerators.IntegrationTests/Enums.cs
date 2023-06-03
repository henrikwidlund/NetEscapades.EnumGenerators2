using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NetEscapades.EnumGenerators.IntegrationTests;

[EnumExtensions]
[EnumJsonConverter(typeof(EnumInNamespaceConverter), CamelCase = true, CaseSensitive = false, PropertyName = nameof(EnumInNamespace))]
[JsonConverter(typeof(EnumInNamespaceConverter))]
public enum EnumInNamespace
{
    First = 0,
    Second = 1,
    Third = 2
}

[EnumExtensions]
[EnumJsonConverter(typeof(EnumWithDisplayNameInNamespaceConverter), CamelCase = true, CaseSensitive = false, PropertyName = nameof(EnumWithDisplayNameInNamespace))]
[JsonConverter(typeof(EnumWithDisplayNameInNamespaceConverter))]
public enum EnumWithDisplayNameInNamespace
{
    First = 0,
    [Display(Name = "2nd")] Second = 1,
    Third = 2
}

[EnumExtensions]
[EnumJsonConverter(typeof(EnumWithDescriptionInNamespaceConverter), CamelCase = true, CaseSensitive = false, PropertyName = nameof(EnumWithDescriptionInNamespace))]
[JsonConverter(typeof(EnumWithDescriptionInNamespaceConverter))]
public enum EnumWithDescriptionInNamespace
{
    First = 0,
    [Description("2nd")] Second = 1,
    Third = 2,
}

[EnumExtensions]
[EnumJsonConverter(typeof(EnumWithSameDisplayNameConverter), CamelCase = true, CaseSensitive = false, PropertyName = nameof(EnumWithSameDisplayName))]
[JsonConverter(typeof(EnumWithSameDisplayNameConverter))]
public enum EnumWithSameDisplayName
{
    First = 0,
    [Display(Name = "2nd")] Second = 1,
    [Display(Name = "2nd")] Third = 2
}

[EnumExtensions]
[EnumJsonConverter(typeof(LongEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = nameof(LongEnum))]
[JsonConverter(typeof(LongEnumConverter))]
public enum LongEnum : long
{
    First = 0,
    Second = 1,
    Third = 2
}

[EnumExtensions]
[EnumJsonConverter(typeof(FlagEnumsConverter), CamelCase = true, CaseSensitive = false, PropertyName = nameof(FlagEnums))]
[JsonConverter(typeof(FlagEnumsConverter))]
[Flags]
public enum FlagEnums
{
    First = 1 << 0,
    Second = 1 << 1,
    Third = 1 << 2,
    Fourth = 1 << 3,
    ThirdAndFourth = Third | Fourth
}
