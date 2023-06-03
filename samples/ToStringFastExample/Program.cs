using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using NetEscapades.EnumGenerators;

const ExampleEnums value = ExampleEnums.First;
Console.WriteLine(value.ToStringFast());

var enumCollection = ExampleEnumsExtensions.GetValues();
var serialized = JsonSerializer.Serialize(enumCollection, new JsonSerializerOptions
{
    WriteIndented = true
});
Console.WriteLine(serialized);
var deserialized = JsonSerializer.Deserialize<ExampleEnums[]>(serialized);
Console.WriteLine($"{nameof(enumCollection)} and {nameof(deserialized)} contains the same items? {deserialized?.SequenceEqual(enumCollection)}");

const FlagEnums flags = FlagEnums.Flag1 | FlagEnums.Flag3;

Console.WriteLine(flags.ToStringFast());
Console.WriteLine($"HasFlag(Flag1), {flags.HasFlagFast(FlagEnums.Flag1)}");
Console.WriteLine($"HasFlag(Flag1), {flags.HasFlagFast(FlagEnums.Flag2)}");
Console.WriteLine($"HasFlag(Flag1), {flags.HasFlagFast(FlagEnums.Flag3)}");

[EnumExtensions]
[EnumJsonConverter(typeof(ExampleEnumsConverter))]
[JsonConverter(typeof(ExampleEnumsConverter))]
internal enum ExampleEnums
{
    [Display(Name = "1st")] First,
    Second,
    Third
}

[EnumExtensions]
[Flags]
internal enum FlagEnums
{
    None = 0,
    Flag1 = 1 << 0,
    Flag2 = 1 << 1,
    Flag3 = 1 << 2
}