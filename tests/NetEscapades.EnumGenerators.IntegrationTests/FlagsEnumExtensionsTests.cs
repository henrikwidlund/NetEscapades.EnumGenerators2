using FluentAssertions;
using Xunit;

namespace NetEscapades.EnumGenerators.IntegrationTests;

public class FlagsEnumExtensionsTests : ExtensionTests<FlagEnums>
{
    public static TheoryData<FlagEnums> ValidEnumValues() => new()
    {
        FlagEnums.First,
        FlagEnums.Second,
        FlagEnums.ThirdAndFourth,
        (FlagEnums) 3
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

    protected override string ToStringFast(FlagEnums value) => value.ToStringFast();

    protected override bool IsDefined(FlagEnums value) => FlagEnumsExtensions.IsDefined(value);

    protected override bool IsDefined(string name, bool allowMatchingMetadataAttribute = false)
        => FlagEnumsExtensions.IsDefined(name, false);

    protected override bool IsDefined(in ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
        => FlagEnumsExtensions.IsDefined(name, false);

    protected override bool TryParse(string name, out FlagEnums parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagEnumsExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override bool TryParse(in ReadOnlySpan<char> name, out FlagEnums parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagEnumsExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override FlagEnums? GetValueOrDefault(string name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagEnumsExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    protected override FlagEnums? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => FlagEnumsExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesToStringFast(FlagEnums value) => GeneratesToStringFastTest(value);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesIsDefined(FlagEnums value) => GeneratesIsDefinedTest(value);

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
            FlagEnums.First,
            FlagEnums.Second,
            FlagEnums.Third,
            FlagEnums.ThirdAndFourth,
            FlagEnums.First | FlagEnums.Second,
            (FlagEnums) 65,
            (FlagEnums) 0
        };

        return from v1 in values
            from v2 in values
            select new object[] { v1, v2 };
    }

    [Theory]
    [MemberData(nameof(AllFlags))]
    public void HasFlags(FlagEnums value, FlagEnums flag)
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
    public void GeneratesGetMetadataNamesOrDefault() => GeneratesGetMetadataNamesOrDefaultTest(FlagEnumsExtensions.GetMetadataNamesOrDefault());

    [Fact]
    public void GeneratesGetValues() => GeneratesGetValuesTest(FlagEnumsExtensions.GetValues());

    [Fact]
    public void GeneratesGetNames() => GeneratesGetNamesTest(FlagEnumsExtensions.GetNames());
}
