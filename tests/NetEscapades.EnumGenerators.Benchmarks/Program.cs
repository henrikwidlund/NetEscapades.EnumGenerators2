using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NetEscapades.EnumGenerators;
using NetEscapades.EnumGenerators.Benchmarks;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

[EnumExtensions]
[EnumJsonConverter(typeof(TestEnumConverter), CaseSensitive = true, AllowMatchingMetadataAttribute = false)]
[JsonConverter(typeof(TestEnumConverter))]
public enum TestEnum
{
    First = 0,
    [Display(Name = "2nd")] Second = 1,
    Third = 2
}

[MemoryDiagnoser]
public class ToStringBenchmark
{
    private const TestEnum BenchmarkEnum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumToString() => BenchmarkEnum.ToString();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string? EnumToStringDisplayNameWithReflection() => EnumHelper.GetDisplayName(BenchmarkEnum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string ToStringFast() => BenchmarkEnum.ToStringFast();
}

[MemoryDiagnoser]
public class IsDefinedBenchmark
{
    private const TestEnum BenchmarkEnum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedName() => Enum.IsDefined(BenchmarkEnum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedName() => TestEnumExtensions.IsDefined(BenchmarkEnum);
}

[MemoryDiagnoser]
public class IsDefinedNameBenchmark
{
    private const string BenchmarkEnum = nameof(TestEnum.Second);
    private const string BenchmarkEnumDisplayName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined() => Enum.IsDefined(typeof(TestEnum), BenchmarkEnum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefined()
        => TestEnumExtensions.IsDefined(BenchmarkEnum, false);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedNameDisplayNameWithReflection()
        => EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, false, out _);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedNameDisplayName()
        => TestEnumExtensions.IsDefined(BenchmarkEnumDisplayName, true);
}

[MemoryDiagnoser]
public class IsDefinedNameFromSpanBenchmark
{
    private const string BenchmarkEnum = nameof(TestEnum.Second);
    private const string BenchmarkEnumDisplayName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined() => Enum.IsDefined(typeof(TestEnum), BenchmarkEnum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedNameDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, false, out _);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefined()
    {
        return TestEnumExtensions.IsDefined(BenchmarkEnum, false);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedSpan()
        => TestEnumExtensions.IsDefined(BenchmarkEnum.AsSpan(), false);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedDisplayNameSpan()
        => TestEnumExtensions.IsDefined(BenchmarkEnumDisplayName.AsSpan(), true);
}

[MemoryDiagnoser]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] EnumGetValues() => Enum.GetValues<TestEnum>();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] ExtensionsGetValues() => TestEnumExtensions.GetValues();
}

[MemoryDiagnoser]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetNames() => Enum.GetNames<TestEnum>();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] ExtensionsGetNames() => TestEnumExtensions.GetNames();
}

[MemoryDiagnoser]
public class TryParseBenchmark
{
    private const string BenchmarkEnum = nameof(TestEnum.Second);
    private const string BenchmarkEnumDisplayName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse() =>
        Enum.TryParse(BenchmarkEnum, false, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParse() =>
        TestEnumExtensions.TryParse(BenchmarkEnum, out var result, false)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, false, out var result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayName() =>
        TestEnumExtensions.TryParse(BenchmarkEnumDisplayName, out var result, false, true)
            ? result
            : default;
}

[MemoryDiagnoser]
public class TryParseFromSpanBenchmark
{
    private const string BenchmarkEnum = nameof(TestEnum.Second);
    private const string BenchmarkEnumDisplayName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse() =>
        Enum.TryParse(BenchmarkEnum, false, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParse() =>
        TestEnumExtensions.TryParse(BenchmarkEnum, out var result, false)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseSpan()
    {
        return TestEnumExtensions.TryParse(BenchmarkEnum.AsSpan(), out var result, false)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, false, out var result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayName() =>
        TestEnumExtensions.TryParse(BenchmarkEnumDisplayName, out var result, false, true)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayNameSpan() =>
        TestEnumExtensions.TryParse(BenchmarkEnumDisplayName.AsSpan(), out var result, false, true)
            ? result
            : default;
}

[MemoryDiagnoser]
public class TryParseIgnoreCaseBenchmark
{
    private const string BenchmarkEnum = "second";
    private const string BenchmarkEnumDisplayName = "2ND";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCase() =>
        Enum.TryParse("second", true, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCase() =>
        TestEnumExtensions.TryParse(BenchmarkEnum, out var result, true)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCaseDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, true, out var result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCaseDisplayName() =>
        TestEnumExtensions.TryParse(BenchmarkEnumDisplayName, out var result, true, true)
            ? result
            : default;
}

[MemoryDiagnoser]
public class TryParseIgnoreCaseFromSpanBenchmark
{
    private const string BenchmarkEnum = "second";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCase() =>
        Enum.TryParse(BenchmarkEnum, true, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCase() =>
        TestEnumExtensions.TryParse(BenchmarkEnum, out var result, true)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCaseSpan() =>
        TestEnumExtensions.TryParse(BenchmarkEnum.AsSpan(), out var result, true)
            ? result
            : default;
}

[MemoryDiagnoser]
public class EnumLengthBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumLength() => Enum.GetNames(typeof(TestEnum)).Length;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumLengthExtensions() => TestEnumExtensions.GetNames().Length;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumLengthProperty() => TestEnumExtensions.Length;
}

[MemoryDiagnoser]
public class GetMetadataNamesOrDefault
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetMetadataNamesOrDefault() => EnumHelper.GetMetadataNamesOrDefault<TestEnum>();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] ExtensionsGetMetadataNamesOrDefault() => TestEnumExtensions.GetMetadataNamesOrDefault();
}

[MemoryDiagnoser]
public class GetValueOrDefault
{
    private const string BenchmarkEnum = nameof(TestEnum.Second);
    private const string BenchmarkEnumDisplayName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefault() =>
        Enum.TryParse(BenchmarkEnum, false, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefault() => TestEnumExtensions.GetValueOrDefault(BenchmarkEnum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, false, out var result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultDisplayName() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnumDisplayName, false, true);
}

[MemoryDiagnoser]
public class GetValueOrDefaultFromSpanBenchmark
{
    private const string BenchmarkEnum = nameof(TestEnum.Second);
    private const string BenchmarkEnumDisplayName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefault() =>
        Enum.TryParse(BenchmarkEnum, false, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefault() => TestEnumExtensions.GetValueOrDefault(BenchmarkEnum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultSpan() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnum.AsSpan());

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, false, out var result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultDisplayName()
    {
        ReadOnlySpan<char> enumAsSpan = BenchmarkEnumDisplayName;
        return TestEnumExtensions.GetValueOrDefault(enumAsSpan.ToString(), false, true);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultDisplayNameSpan() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnumDisplayName.AsSpan(), false, true);
}

[MemoryDiagnoser]
public class GetValueOrDefaultIgnoreCaseBenchmark
{
    private const string BenchmarkEnum = "second";
    private const string BenchmarkEnumDisplayName = "2ND";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumGetValueOrDefaultIgnoreCase() =>
        Enum.TryParse(BenchmarkEnum, true, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCase() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnum, true);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultIgnoreCaseDisplayNameWithReflection() =>
        EnumHelper.TryParseByDisplayName<TestEnum>(BenchmarkEnumDisplayName, true, out var result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCaseDisplayName() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnumDisplayName, true, true);
}

[MemoryDiagnoser]
public class GetValueOrDefaultIgnoreCaseFromSpanBenchmark
{
    private const string BenchmarkEnum = "second";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultIgnoreCase() =>
        Enum.TryParse(BenchmarkEnum, true, out TestEnum result)
            ? result
            : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCase() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnum, true);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCaseSpan() =>
        TestEnumExtensions.GetValueOrDefault(BenchmarkEnum.AsSpan(), true);
}

[MemoryDiagnoser]
public class DeserializeBenchmark
{
    private const string EnumsString = """
            ["Second","Third","First","Second"]
            """;

    private static readonly Memory<char> EnumsMemory = EnumsString.ToArray().AsMemory();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? JsonStringEnumConverter() =>
        JsonSerializer.Deserialize<TestEnum[]>(EnumsString, JsonSerializerOptions);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? JsonStringEnumConverterSpan() =>
        JsonSerializer.Deserialize<TestEnum[]>(EnumsMemory.Span, JsonSerializerOptions);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? EnumJsonConverter() => JsonSerializer.Deserialize<TestEnum[]>(EnumsString);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? EnumJsonConverterSpan() => JsonSerializer.Deserialize<TestEnum[]>(EnumsMemory.Span);
}

[EnumExtensions]
[EnumJsonConverter(typeof(TestEnumIgnoreCaseConverter), CaseSensitive = false, AllowMatchingMetadataAttribute = false)]
[JsonConverter(typeof(TestEnumIgnoreCaseConverter))]
public enum TestEnumIgnoreCase
{
    First = 0,
    [Display(Name = "2nd")] Second = 1,
    Third = 2
}

[MemoryDiagnoser]
public class DeserializeIgnoreCaseBenchmark
{
    private const string EnumsString = """
            ["second","Third","first","Second"]
            """;

    private static readonly char[] EnumsChars = EnumsString.ToArray();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? JsonStringEnumConverter() =>
        JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(EnumsString, JsonSerializerOptions);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? JsonStringEnumConverterSpan() =>
        JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(EnumsChars.AsSpan(), JsonSerializerOptions);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? EnumJsonConverter() => JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(EnumsString);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? EnumJsonConverterSpan() =>
        JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(EnumsChars.AsSpan());
}

[MemoryDiagnoser]
public class SerializeBenchmark
{
    private static readonly TestEnum[] BenchmarkEnums =
    {
        TestEnum.Second,
        TestEnum.Third,
        TestEnum.First,
        TestEnum.Second
    };

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string JsonStringEnumConverter() => JsonSerializer.Serialize(BenchmarkEnums, JsonSerializerOptions);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumJsonConverter() => JsonSerializer.Serialize(BenchmarkEnums);
}
