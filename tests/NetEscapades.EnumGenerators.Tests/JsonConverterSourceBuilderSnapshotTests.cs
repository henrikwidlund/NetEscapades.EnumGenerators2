using System.Text;

namespace NetEscapades.EnumGenerators.Tests;

[UsesVerify]
public class JsonConverterSourceBuilderSnapshotTests
{
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
        var result = JsonConverterSourceBuilder.GenerateJsonConverterClass(sb, value);

        return Verify(result)
            .UseDirectory("Snapshots");
    }
}
