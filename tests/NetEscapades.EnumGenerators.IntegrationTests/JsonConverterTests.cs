using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace NetEscapades.EnumGenerators.IntegrationTests;

public class JsonConverterTests
{
    public static TheoryData<string> ValuesToParse() => new()
    {
        "First",
        "Second",
        "2nd",
        "2ND",
        "first",
        "SECOND",
        "3",
        "267",
        "-267",
        "2147483647",
        "3000000000",
        "Fourth",
        "Fifth"
    };

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratedJsonConverterEnumInNamespace(string name)
    {
        var modelWithString = new ModelWithString(name);
        var json = JsonSerializer.Serialize(modelWithString);

        if (EnumInNamespaceExtensions.TryParse(name.AsSpan(), out var parsed, true, true))
        {
            var modelWithEnum = JsonSerializer.Deserialize<ModelWithEnum<EnumInNamespace>>(json);
            modelWithEnum.Should().NotBeNull();
            modelWithEnum.EnumProperty.Should().Be(parsed);
        }
        else
        {
            var action = () => JsonSerializer.Deserialize<ModelWithEnum<EnumInNamespace>>(json);
            action.Should().Throw<JsonException>();
        }
    }

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratedJsonConverterEnumWithDisplayNameInNamespace(string name)
    {
        var modelWithString = new ModelWithString(name);
        var json = JsonSerializer.Serialize(modelWithString);

        if (EnumWithDisplayNameInNamespaceExtensions.TryParse(name.AsSpan(), out var parsed, true, true))
        {
            var modelWithEnum = JsonSerializer.Deserialize<ModelWithEnum<EnumWithDisplayNameInNamespace>>(json);
            modelWithEnum.Should().NotBeNull();
            modelWithEnum.EnumProperty.Should().Be(parsed);
        }
        else
        {
            var action = () => JsonSerializer.Deserialize<ModelWithEnum<EnumWithDisplayNameInNamespace>>(json);
            action.Should().Throw<JsonException>();
        }
    }

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratedJsonConverterEnumWithSameDisplayName(string name)
    {
        var modelWithString = new ModelWithString(name);
        var json = JsonSerializer.Serialize(modelWithString);

        if (EnumWithSameDisplayNameExtensions.TryParse(name.AsSpan(), out var parsed, true, true))
        {
            var modelWithEnum = JsonSerializer.Deserialize<ModelWithEnum<EnumWithSameDisplayName>>(json);
            modelWithEnum.Should().NotBeNull();
            modelWithEnum.EnumProperty.Should().Be(parsed);
        }
        else
        {
            var action = () => JsonSerializer.Deserialize<ModelWithEnum<EnumWithSameDisplayName>>(json);
            action.Should().Throw<JsonException>();
        }
    }

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratedJsonConverterFlagsEnum(string name)
    {
        var modelWithString = new ModelWithString(name);
        var json = JsonSerializer.Serialize(modelWithString);

        if (FlagEnumsExtensions.TryParse(name.AsSpan(), out var parsed, true, true))
        {
            var modelWithEnum = JsonSerializer.Deserialize<ModelWithEnum<FlagEnums>>(json);
            modelWithEnum.Should().NotBeNull();
            modelWithEnum.EnumProperty.Should().Be(parsed);
        }
        else
        {
            var action = () => JsonSerializer.Deserialize<ModelWithEnum<FlagEnums>>(json);
            action.Should().Throw<JsonException>();
        }
    }

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratedJsonConverterLongEnum(string name)
    {
        var modelWithString = new ModelWithString(name);
        var json = JsonSerializer.Serialize(modelWithString);

        if (LongEnumExtensions.TryParse(name.AsSpan(), out var parsed, true, true))
        {
            var modelWithEnum = JsonSerializer.Deserialize<ModelWithEnum<LongEnum>>(json);
            modelWithEnum.Should().NotBeNull();
            modelWithEnum.EnumProperty.Should().Be(parsed);
        }
        else
        {
            var action = () => JsonSerializer.Deserialize<ModelWithEnum<LongEnum>>(json);
            action.Should().Throw<JsonException>();
        }
    }

    private sealed record ModelWithString(string EnumProperty);

    private sealed record ModelWithEnum<TEnum>(TEnum EnumProperty)
        where TEnum : struct, Enum;
}
