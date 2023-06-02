using FluentAssertions;
using Xunit;

namespace NetEscapades.EnumGenerators.IntegrationTests;

public class FlagsEnumExtensionsTests : ExtensionTests<FlagsEnum>
{
    public static TheoryData<FlagsEnum> ValidEnumValues() => new()
    {
        FlagsEnum.First,
        FlagsEnum.Second,
        FlagsEnum.ThirdAndFourth,
        (FlagsEnum)3
    };

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

    protected override string ToStringFast(FlagsEnum value) => value.ToStringFast();

    protected override bool IsDefined(FlagsEnum value) => FlagsEnumExtensions.IsDefined(value);

    protected override bool IsDefined(string name, bool allowMatchingMetadataAttribute = false)
        => FlagsEnumExtensions.IsDefined(name, false);

    protected override bool IsDefined(in ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
        => FlagsEnumExtensions.IsDefined(name, false);

    protected override bool TryParse(string name, out FlagsEnum parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagsEnumExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override bool TryParse(in ReadOnlySpan<char> name, out FlagsEnum parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagsEnumExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override FlagsEnum? GetValueOrDefault(string name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagsEnumExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    protected override FlagsEnum? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagsEnumExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesToStringFast(FlagsEnum value) => GeneratesToStringFastTest(value);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesIsDefined(FlagsEnum value) => GeneratesIsDefinedTest(value);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingName(string name) => GeneratesIsDefinedTest(name, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingNameAsSpan(string name) => GeneratesIsDefinedTest(name.AsSpan(), false);

    public static IEnumerable<object[]> AllFlags()
    {
        var values = new[]
        {
            FlagsEnum.First,
            FlagsEnum.Second,
            FlagsEnum.Third,
            FlagsEnum.ThirdAndFourth,
            FlagsEnum.First | FlagsEnum.Second,
            (FlagsEnum)65,
            (FlagsEnum)0,
        };

        return from v1 in values
            from v2 in values
            select new object[] { v1, v2 };
    }

    [Theory]
    [MemberData(nameof(AllFlags))]
    public void HasFlags(FlagsEnum value, FlagsEnum flag)
    {
        var isDefined = value.HasFlagFast(flag);

        isDefined.Should().Be(value.HasFlag(flag));
    }

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParse(string name) => GeneratesTryParseTest(name, false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseAsSpan(string name) => GeneratesTryParseTest(name.AsSpan(), false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCase(string name) => GeneratesTryParseTest(name, true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCaseAsSpan(string name)
        => GeneratesTryParseTest(name.AsSpan(), true, false);


    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefault(string name) => GeneratesGetValueOrDefaultTest(name, false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultAsSpan(string name) => GeneratesGetValueOrDefaultTest(name.AsSpan(), false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCase(string name) => GeneratesGetValueOrDefaultTest(name, true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCaseAsSpan(string name)
        => GeneratesGetValueOrDefaultTest(name.AsSpan(), true, false);

    [Fact]
    public void GeneratesGetMetadataNamesOrDefault() => GeneratesGetMetadataNamesOrDefaultTest(FlagsEnumExtensions.GetMetadataNamesOrDefault());

    [Fact]
    public void GeneratesGetValues() => GeneratesGetValuesTest(FlagsEnumExtensions.GetValues());

    [Fact]
    public void GeneratesGetNames() => GeneratesGetNamesTest(FlagsEnumExtensions.GetNames());
}