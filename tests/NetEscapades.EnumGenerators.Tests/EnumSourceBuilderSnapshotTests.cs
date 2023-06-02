using System.Text;

namespace NetEscapades.EnumGenerators.Tests;

[UsesVerify]
public class EnumSourceBuilderSnapshotTests
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
        var result = EnumSourceBuilder.GenerateExtensionClass(sb, value);

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
        var result = EnumSourceBuilder.GenerateExtensionClass(sb, value);

        return Verify(result)
            .UseDirectory("Snapshots");
    }
}
