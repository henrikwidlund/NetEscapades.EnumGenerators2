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
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumToString()
    {
        return _enum.ToString();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string? EnumToStringDisplayNameWithReflection()
    {
        return EnumHelper.GetDisplayName(_enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string ToStringFast()
    {
        return _enum.ToStringFast();
    }
}

[MemoryDiagnoser]
public class IsDefinedBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedName()
    {
        return Enum.IsDefined(typeof(TestEnum), _enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedName()
    {
        return TestEnumExtensions.IsDefined(_enum);
    }
}

[MemoryDiagnoser]
public class IsDefinedNameBenchmark
{
    private static readonly string _enum = nameof(TestEnum.Second);
    private static readonly string _enumDisplaName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined()
    {
        return Enum.IsDefined(typeof(TestEnum), _enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefined()
    {
        return TestEnumExtensions.IsDefined(_enum, false);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedNameDisplayNameWithReflection()
    {
        return EnumHelper.TryParseByDisplayName<TestEnum>(_enumDisplaName, false, out _);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedNameDisplayName()
    {
        return TestEnumExtensions.IsDefined(_enumDisplaName, true);
    }
}

[MemoryDiagnoser]
public class IsDefinedNameFromSpanBenchmark
{
    private static readonly char[] _enum = { 'S', 'e', 'c', 'o', 'n', 'd' };
    private static readonly char[] _enumDisplayName = { '2', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return Enum.IsDefined(typeof(TestEnum), enumAsSpan.ToString());
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedNameDisplayNameWithReflection()
    {
        ReadOnlySpan<char> enumAsSpan = _enumDisplayName;
        return EnumHelper.TryParseByDisplayName<TestEnum>(enumAsSpan.ToString(), false, out _);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefined()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return TestEnumExtensions.IsDefined(enumAsSpan.ToString(), false);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedSpan()
    {
        return TestEnumExtensions.IsDefined(_enum.AsSpan(), false);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedDisplayNameSpan()
    {
        return TestEnumExtensions.IsDefined(_enumDisplayName.AsSpan(), true);
    }
}

[MemoryDiagnoser]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] EnumGetValues()
    {
        return Enum.GetValues<TestEnum>();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] ExtensionsGetValues()
    {
        return TestEnumExtensions.GetValues();
    }
}

[MemoryDiagnoser]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetNames()
    {
        return Enum.GetNames<TestEnum>();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] ExtensionsGetNames()
    {
        return TestEnumExtensions.GetNames();
    }
}

[MemoryDiagnoser]
public class TryParseBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse()
    {
        return Enum.TryParse("Second", false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParse()
    {
        return TestEnumExtensions.TryParse("Second", out var result, false)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseDisplayNameWithReflection()
    {
        return EnumHelper.TryParseByDisplayName<TestEnum>("2nd", false, out var result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayName()
    {
        return TestEnumExtensions.TryParse("2nd", out var result, false, true)
            ? result
            : default;
    }
}

[MemoryDiagnoser]
public class TryParseFromSpanBenchmark
{
    private static readonly char[] _enum = { 'S', 'e', 'c', 'o', 'n', 'd' };
    private static readonly char[] _enumDisplayName = { '2', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return Enum.TryParse(enumAsSpan.ToString(), false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParse()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return TestEnumExtensions.TryParse(enumAsSpan.ToString(), out var result, false)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseSpan()
    {
        return TestEnumExtensions.TryParse(_enum.AsSpan(), out var result, false)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseDisplayNameWithReflection()
    {
        ReadOnlySpan<char> enumAsSpan = _enumDisplayName;
        return EnumHelper.TryParseByDisplayName<TestEnum>(enumAsSpan.ToString(), ignoreCase: false, out var result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayName()
    {
        ReadOnlySpan<char> enumAsSpan = _enumDisplayName;
        return TestEnumExtensions.TryParse(enumAsSpan.ToString(), out var result, ignoreCase: false, allowMatchingMetadataAttribute: true)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayNameSpan()
    {
        return TestEnumExtensions.TryParse(_enumDisplayName.AsSpan(), out var result, ignoreCase: false, allowMatchingMetadataAttribute: true)
            ? result
            : default;
    }
}

[MemoryDiagnoser]
public class TryParseIgnoreCaseBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCase()
    {
        return Enum.TryParse("second", true, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCase()
    {
        return TestEnumExtensions.TryParse("second", out var result, true)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCaseDisplayNameWithReflection()
    {
        return EnumHelper.TryParseByDisplayName<TestEnum>("2ND", true, out var result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCaseDisplayName()
    {
        return TestEnumExtensions.TryParse("2ND", out var result, true, true)
            ? result
            : default;
    }
}

[MemoryDiagnoser]
public class TryParseIgnoreCaseFromSpanBenchmark
{
    private static readonly char[] _enum = { 's', 'e', 'c', 'o', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCase()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return Enum.TryParse(enumAsSpan.ToString(), true, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCase()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return TestEnumExtensions.TryParse(enumAsSpan.ToString(), out var result, true)
            ? result
            : default;
    }


    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCaseSpan()
    {
        return TestEnumExtensions.TryParse(_enum.AsSpan(), out var result, ignoreCase: true)
            ? result
            : default;
    }
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
    public string[] EnumGetMetadataNamesOrDefault()
    {
        return EnumHelper.GetMetadataNamesOrDefault<TestEnum>();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] ExtensionsGetMetadataNamesOrDefault()
    {
        return TestEnumExtensions.GetMetadataNamesOrDefault();
    }
}

[MemoryDiagnoser]
public class GetValueOrDefault
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefault()
    {
        return Enum.TryParse("Second", false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefault()
    {
        return TestEnumExtensions.GetValueOrDefault("Second");
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultDisplayNameWithReflection()
    {
        return EnumHelper.TryParseByDisplayName<TestEnum>("2nd", false, out var result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultDisplayName()
    {
        return TestEnumExtensions.GetValueOrDefault("2nd", false, true);
    }
}

[MemoryDiagnoser]
public class GetValueOrDefaultFromSpanBenchmark
{
    private static readonly char[] _enum = { 'S', 'e', 'c', 'o', 'n', 'd' };
    private static readonly char[] _enumDisplayName = { '2', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefault()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return Enum.TryParse(enumAsSpan.ToString(), false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefault()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return TestEnumExtensions.GetValueOrDefault(enumAsSpan.ToString());
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultSpan()
    {
        return TestEnumExtensions.GetValueOrDefault(_enum.AsSpan());
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultDisplayNameWithReflection()
    {
        ReadOnlySpan<char> enumAsSpan = _enumDisplayName;
        return EnumHelper.TryParseByDisplayName<TestEnum>(enumAsSpan.ToString(), false, out var result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultDisplayName()
    {
        ReadOnlySpan<char> enumAsSpan = _enumDisplayName;
        return TestEnumExtensions.GetValueOrDefault(enumAsSpan.ToString(), false, true);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultDisplayNameSpan()
    {
        return TestEnumExtensions.GetValueOrDefault(_enumDisplayName.AsSpan(), false, true);
    }
}

[MemoryDiagnoser]
public class GetValueOrDefaultIgnoreCaseBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumGetValueOrDefaultIgnoreCase()
    {
        return Enum.TryParse("second", true, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCase()
    {
        return TestEnumExtensions.GetValueOrDefault("second", true);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultIgnoreCaseDisplayNameWithReflection()
    {
        return EnumHelper.TryParseByDisplayName<TestEnum>("2ND", true, out var result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCaseDisplayName()
    {
        return TestEnumExtensions.GetValueOrDefault("2ND", true, true);
    }
}

[MemoryDiagnoser]
public class GetValueOrDefaultIgnoreCaseFromSpanBenchmark
{
    private static readonly char[] _enum = { 's', 'e', 'c', 'o', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? EnumGetValueOrDefaultIgnoreCase()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return Enum.TryParse(enumAsSpan.ToString(), true, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCase()
    {
        ReadOnlySpan<char> enumAsSpan = _enum;
        return TestEnumExtensions.GetValueOrDefault(enumAsSpan.ToString(), true);
    }


    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum? ExtensionsGetValueOrDefaultIgnoreCaseSpan()
    {
        return TestEnumExtensions.GetValueOrDefault(_enum.AsSpan(), true);
    }
}

[MemoryDiagnoser]
public class DeserializeBenchmark
{
    private static readonly string _enumsString =
        """
        ["Second","Third","First","Second"]
        """;

    private static readonly Memory<char> _enumsMemory = _enumsString.ToArray().AsMemory();

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? JsonStringEnumConverter()
    {
        return JsonSerializer.Deserialize<TestEnum[]>(_enumsString, _jsonSerializerOptions);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? JsonStringEnumConverterSpan()
    {
        return JsonSerializer.Deserialize<TestEnum[]>(_enumsMemory.Span, _jsonSerializerOptions);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? EnumJsonConverter()
    {
        return JsonSerializer.Deserialize<TestEnum[]>(_enumsString);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[]? EnumJsonConverterSpan()
    {
        return JsonSerializer.Deserialize<TestEnum[]>(_enumsMemory.Span);
    }
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
    private static readonly string _enumsString =
        """
        ["second","Third","first","second"]
        """;

    private static readonly Memory<char> _enumsMemory = _enumsString.ToArray().AsMemory();

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? JsonStringEnumConverter()
    {
        return JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(_enumsString, _jsonSerializerOptions);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? JsonStringEnumConverterSpan()
    {
        return JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(_enumsMemory.Span, _jsonSerializerOptions);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? EnumJsonConverter()
    {
        return JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(_enumsString);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnumIgnoreCase[]? EnumJsonConverterSpan()
    {
        return JsonSerializer.Deserialize<TestEnumIgnoreCase[]>(_enumsMemory.Span);
    }
}



[MemoryDiagnoser]
public class SerializeBenchmark
{
    private static readonly TestEnum[] _enums = {
        TestEnum.Second,
        TestEnum.Third,
        TestEnum.First,
        TestEnum.Second
    };

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string JsonStringEnumConverter()
    {
        return JsonSerializer.Serialize(_enums, _jsonSerializerOptions);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumJsonConverter()
    {
        return JsonSerializer.Serialize(_enums);
    }
}
