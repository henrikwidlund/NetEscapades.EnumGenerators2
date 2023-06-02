using System.Text;

namespace NetEscapades.EnumGenerators.Tests;

[UsesVerify]
public class SourceGenerationHelperSnapshotTests
{
    [Fact]
    public Task GeneratesEnumCorrectly()
    {
        var value = new EnumToGenerate(
            "ShortNameExtensions",
            "Something.Blah.ShortName",
            "Something.Blah",
            true,
            false,
            "int",
            false,
            new EquatableArray<(string Key, EnumValueOption Value)>(
                new[]
                {
                    ("First", new EnumValueOption(null, false)),
                    ("Second", new EnumValueOption(null, false))
                }));

        var sb = new StringBuilder();
        var result = SourceGenerationHelper.GenerateExtensionClass(sb, value);

        return Verify(result)
            .UseDirectory("Snapshots");
    }

    [Fact]
    public Task GeneratesFlagsEnumCorrectly()
    {
        var value = new EnumToGenerate(
            "ShortNameExtensions",
            "Something.Blah.ShortName",
            "Something.Blah",
            true,
            true,
            "int",
            false,
            new EquatableArray<(string Key, EnumValueOption Value)>(
                new[]
                {
                    ("First", new EnumValueOption(null, false)),
                    ("Second", new EnumValueOption(null, false))
                }));

        var sb = new StringBuilder();
        var result = SourceGenerationHelper.GenerateExtensionClass(sb, value);

        return Verify(result)
            .UseDirectory("Snapshots");
    }

    [Fact]
    public Task GeneratesJsonConverterCorrectly()
    {
        var value = new JsonConverterToGenerate
        {
            CamelCase = true,
            CaseSensitive = false,
            ConverterNamespace = "Something.Blah",
            ConverterType = "ShortNameConverter",
            ExtensionName = "ShortNameExtensions",
            ExtensionNamespace = "Something.Blah",
            IsPublic = true,
            PropertyName = "ShortName",
            FullyQualifiedName = "Something.Blah.ShortNameConverter"
        };

        var sb = new StringBuilder();
        var result = SourceGenerationHelper.GenerateJsonConverterClass(sb, value);

        return Verify(result)
            .UseDirectory("Snapshots");
    }
}
