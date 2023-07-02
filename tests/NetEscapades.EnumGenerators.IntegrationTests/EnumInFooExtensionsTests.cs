using Foo;
using Xunit;

namespace NetEscapades.EnumGenerators.IntegrationTests;

public class EnumInFooExtensionsTests : ExtensionTests<EnumInFoo>
{
    public static TheoryData<EnumInFoo> ValidEnumValues() => new()
    {
        EnumInFoo.First,
        EnumInFoo.Second,
        (EnumInFoo) 3
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

    protected override string ToStringFast(EnumInFoo value) => value.ToStringFast();

    protected override bool IsDefined(EnumInFoo value) => EnumInFooExtensions.IsDefined(value);

    protected override bool IsDefined(string name, bool allowMatchingMetadataAttribute = false) => EnumInFooExtensions.IsDefined(name, allowMatchingMetadataAttribute);

    protected override bool IsDefined(in ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute) => EnumInFooExtensions.IsDefined(name, allowMatchingMetadataAttribute);

    protected override bool TryParse(string name, out EnumInFoo parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInFooExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override bool TryParse(in ReadOnlySpan<char> name, out EnumInFoo parsed, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInFooExtensions.TryParse(name, out parsed, ignoreCase, allowMatchingMetadataAttribute);

    protected override EnumInFoo? GetValueOrDefault(string name, bool ignoreCase, bool allowMatchingMetadataAttribute)
        => EnumInFooExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    protected override EnumInFoo? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase,
        bool allowMatchingMetadataAttribute)
        => EnumInFooExtensions.GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesToStringFast(EnumInFoo value) => GeneratesToStringFastTest(value);

    [Theory]
    [MemberData(nameof(ValidEnumValues))]
    public void GeneratesIsDefined(EnumInFoo value) => GeneratesIsDefinedTest(value);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingName(string name) => GeneratesIsDefinedTest(name, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingNameAsSpan(string name) => GeneratesIsDefinedTest(name.AsSpan(), false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingNameAllowMatchingMetadataAttribute(string name) => GeneratesIsDefinedTest(name, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesIsDefinedUsingNameAllowMatchingMetadataAttributeAsSpan(string name) => GeneratesIsDefinedTest(name.AsSpan(), true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParse(string name) => GeneratesTryParseTest(name, false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseAsSpan(string name) => GeneratesTryParseTest(name.AsSpan(), false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseAllowMatchingMetadataAttribute(string name) => GeneratesTryParseTest(name, false, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseAllowMatchingMetadataAttributeAsSpan(string name) => GeneratesTryParseTest(name.AsSpan(), false, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCase(string name) => GeneratesTryParseTest(name, true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCaseAsSpan(string name) => GeneratesTryParseTest(name.AsSpan(), true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCaseAllowMatchingMetadataAttribute(string name) => GeneratesTryParseTest(name, true, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesTryParseIgnoreCaseAllowMatchingMetadataAttributeAsSpan(string name) => GeneratesTryParseTest(name.AsSpan(), true, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefault(string name) => GeneratesGetValueOrDefaultTest(name, false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultAsSpan(string name)
        => GeneratesGetValueOrDefaultTest(name.AsSpan(), false, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultAllowMatchingMetadataAttribute(string name)
        => GeneratesGetValueOrDefaultTest(name, false, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultAllowMatchingMetadataAttributeAsSpan(string name)
        => GeneratesGetValueOrDefaultTest(name.AsSpan(), false, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCase(string name)
        => GeneratesGetValueOrDefaultTest(name, true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCaseAsSpan(string name)
        => GeneratesGetValueOrDefaultTest(name.AsSpan(), true, false);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCaseAllowMatchingMetadataAttribute(string name)
        => GeneratesGetValueOrDefaultTest(name, true, true);

    [Theory]
    [MemberData(nameof(ValuesToParse))]
    public void GeneratesGetValueOrDefaultIgnoreCaseAllowMatchingMetadataAttributeAsSpan(string name)
        => GeneratesGetValueOrDefaultTest(name.AsSpan(), true, true);

    [Fact]
    public void GeneratesGetMetadataNamesOrDefault()
        => GeneratesGetMetadataNamesOrDefaultTest(EnumInFooExtensions.GetMetadataNamesOrDefault());

    [Fact]
    public void GeneratesGetValues() => GeneratesGetValuesTest(EnumInFooExtensions.GetValues());

    [Fact]
    public void GeneratesGetNames() => GeneratesGetNamesTest(EnumInFooExtensions.GetNames());
}
