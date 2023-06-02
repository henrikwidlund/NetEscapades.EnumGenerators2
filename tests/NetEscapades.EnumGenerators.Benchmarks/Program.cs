using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NetEscapades.EnumGenerators;
using NetEscapades.EnumGenerators.Benchmarks;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

[EnumExtensions]
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
    public string EnumToStringDisplayNameWithReflection()
    {
        return EnumHelper<TestEnum>.GetDisplayName(_enum);
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
        return EnumHelper<TestEnum>.TryParseByDisplayName(_enumDisplaName, false, out _);
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
        return EnumHelper<TestEnum>.TryParseByDisplayName(enumAsSpan.ToString(), false, out _);
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
        return EnumHelper<TestEnum>.TryParseByDisplayName("2nd", false, out var result) ? result : default;
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
        return EnumHelper<TestEnum>.TryParseByDisplayName(enumAsSpan.ToString(), ignoreCase: false, out var result) ? result : default;
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
        return EnumHelper<TestEnum>.TryParseByDisplayName("2ND", true, out var result) ? result : default;
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
