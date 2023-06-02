namespace NetEscapades.EnumGenerators.Tests;

[UsesVerify]
public class JsonConverterGeneratorTests
{
    [Fact]
    public Task CanGenerateEnumExtensionsInGlobalNamespace()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

[EnumExtensions]
[EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
[JsonConverter(typeof(MyEnumConverter))]
public enum MyEnum
{
    First,
    Second
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsInChildNamespace()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

namespace MyTestNameSpace
{
    [EnumExtensions]
    [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
    [JsonConverter(typeof(MyEnumConverter))]
    public enum MyEnum
    {
        First = 0,
        Second = 1
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsInNestedClass()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

namespace MyTestNameSpace
{
    public class InnerClass
    {
        [EnumExtensions]
        [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
        [JsonConverter(typeof(MyEnumConverter))]
        internal enum MyEnum
        {
            First = 0,
            Second = 1
        }
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsWithCustomName()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

namespace MyTestNameSpace
{
    [EnumExtensions(ExtensionClassName = ""A"")]
    [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
    [JsonConverter(typeof(MyEnumConverter))]
    internal enum MyEnum
    {
        First = 0,
        Second = 1
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsWithCustomNamespace()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

namespace MyTestNameSpace
{
    [EnumExtensions(ExtensionClassNamespace = ""A.B"")]
    [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
    [JsonConverter(typeof(MyEnumConverter))]
    internal enum MyEnum
    {
        First = 0,
        Second = 1
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsWithCustomNamespaceAndName()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

namespace MyTestNameSpace
{
    [EnumExtensions(ExtensionClassNamespace = ""A.B"", ExtensionClassName = ""C"")]
    [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
    [JsonConverter(typeof(MyEnumConverter))]
    internal enum MyEnum
    {
        First = 0,
        Second = 1
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsWithDisplayName()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;
using System.ComponentModel.DataAnnotations;

namespace MyTestNameSpace
{
    [EnumExtensions]
    [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
    [JsonConverter(typeof(MyEnumConverter))]
    public enum MyEnum
    {
        First = 0,

        [Display(Name = ""2nd"")]
        Second = 1,
        Third = 2,

        [Display(Name = ""4th"")]
        Fourth = 3
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task CanGenerateEnumExtensionsWithSameDisplayName()
    {
        const string input = @"using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;
using System.ComponentModel.DataAnnotations;

namespace MyTestNameSpace
{
    [EnumExtensions]
    [EnumJsonConverterAttribute(typeof(MyEnumConverter), CamelCase = true, CaseSensitive = false, PropertyName = ""bla"")]
    [JsonConverter(typeof(MyEnumConverter))]
    public enum MyEnum
    {
        First = 0,

        [Display(Name = ""2nd"")]
        Second = 1,
        Third = 2,

        [Display(Name = ""2nd"")]
        Fourth = 3
    }
}";
        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<GenerateJsonConverter>(input);

        Assert.Empty(diagnostics);
        return Verify(output).UseDirectory("Snapshots");
    }
}
