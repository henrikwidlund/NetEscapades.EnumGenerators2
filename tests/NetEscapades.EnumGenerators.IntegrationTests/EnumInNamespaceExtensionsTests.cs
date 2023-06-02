using Xunit;

namespace NetEscapades.EnumGenerators.IntegrationTests;

public class EnumInNamespaceExtensionsTests : ExtensionTests<EnumInNamespace>
{
    public static TheoryData<EnumInNamespace> ValidEnumValues() => new()
    {
        EnumInNamespace.First,
        EnumInNamespace.Second,
        (EnumInNamespace)3
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

    protected override string ToStringFast(EnumInNamespace value) => value.ToStringFast();

    protected override bool IsDefined(EnumInNamespace value) => EnumInNamespaceExtensions.IsDefined(value);

    protected override bool IsDefined(string name, bool allowMatchingMetadataAttribute = false)
        => EnumInNamespaceExtensions.IsDefined(name, false);

    protected override bool IsDefined(in ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
        => EnumInNamespaceExtensions.IsDefined(name, false);

    protected override bool TryParse(string name, out EnumInNamespace parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInNamespaceExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override bool TryParse(in ReadOnlySpan<char> name, out EnumInNamespace parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInNamespaceExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override EnumInNamespace? GetValueOrDefault(string name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInNamespaceExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    protected override EnumInNamespace? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInNamespaceExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesToStringFast(EnumInNamespace value) => GeneratesToStringFastTest(value);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesIsDefined(EnumInNamespace value) => GeneratesIsDefinedTest(value);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingName(string name) => GeneratesIsDefinedTest(name, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingNameAsSpan(string name) => GeneratesIsDefinedTest(name.AsSpan(), false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParse(string name) => GeneratesTryParseTest(name, false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseUsingSpan(string name) => GeneratesTryParseTest(name.AsSpan(), false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCase(string name) => GeneratesTryParseTest(name, true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefault(string name) => GeneratesGetValueOrDefaultTest(name, false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultUsingSpan(string name) => GeneratesGetValueOrDefaultTest(name.AsSpan(), false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCase(string name) => GeneratesGetValueOrDefaultTest(name, true, false);

    [Fact]
    public void GeneratesGetMetadataNamesOrDefault() => GeneratesGetMetadataNamesOrDefaultTest(EnumInNamespaceExtensions.GetMetadataNamesOrDefault());

    [Fact]
    public void GeneratesGetValues() => GeneratesGetValuesTest(EnumInNamespaceExtensions.GetValues());

    [Fact]
    public void GeneratesGetNames() => GeneratesGetNamesTest(EnumInNamespaceExtensions.GetNames());
}