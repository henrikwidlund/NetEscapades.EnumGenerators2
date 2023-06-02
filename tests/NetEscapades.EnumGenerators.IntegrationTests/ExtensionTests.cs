using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;

namespace NetEscapades.EnumGenerators.IntegrationTests;

#nullable enable
public abstract class ExtensionTests<T> where T : struct, Enum
{
    protected abstract string ToStringFast(T value);
    protected abstract bool IsDefined(T value);
    protected abstract bool IsDefined(string name, bool allowMatchingMetadataAttribute = false);
    protected abstract bool IsDefined(in ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute);
    protected abstract bool TryParse(string name, out T parsed, bool ignoreCase, bool allowMatchingMetadataAttribute);
    protected abstract bool TryParse(in ReadOnlySpan<char> name, out T parsed, bool ignoreCase, bool allowMatchingMetadataAttribute);
    protected abstract T? GetValueOrDefault(string name, bool ignoreCase, bool allowMatchingMetadataAttribute);
    protected abstract T? GetValueOrDefault(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute);

    protected void GeneratesToStringFastTest(T value)
    {
        var serialized = ToStringFast(value);
        var valueAsString = value.ToString();

        TryGetDisplayNameOrDescription(valueAsString, out var displayName);
        var expectedValue = displayName ?? valueAsString;

        serialized.Should().Be(expectedValue);
    }

    protected void GeneratesIsDefinedTest(T value)
    {
        var isDefined = IsDefined(value);

        isDefined.Should().Be(Enum.IsDefined(typeof(T), value));
    }

    protected void GeneratesIsDefinedTest(string name, bool allowMatchingMetadataAttribute)
    {
        var isDefined = IsDefined(name, allowMatchingMetadataAttribute);
        var expectedResult = ValidateIsDefined(name, allowMatchingMetadataAttribute);
        isDefined.Should().Be(expectedResult);
    }

    protected void GeneratesIsDefinedTest(in ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
    {
        var isDefined = IsDefined(name, allowMatchingMetadataAttribute);
        var expectedResult = ValidateIsDefined(name.ToString(), allowMatchingMetadataAttribute);
        isDefined.Should().Be(expectedResult);
    }

    private bool ValidateIsDefined(string name, bool allowMatchingMetadataAttribute)
    {
        bool expectedResult;
        if (allowMatchingMetadataAttribute)
        {
            expectedResult = TryGetEnumByDisplayNameOrDescription(name, false, out _);
            if (!expectedResult)
            {
                expectedResult = Enum.IsDefined(typeof(T), name);
            }
        }
        else
        {
            expectedResult = Enum.IsDefined(typeof(T), name);
        }

        return expectedResult;
    }

    protected void GeneratesTryParseTest(string name, bool ignoreCase, bool allowMatchingMetadataAttribute)
    {
        var isValid = TryParse(name, out var result, ignoreCase, allowMatchingMetadataAttribute);
        ValidateTryParse(name, ignoreCase, allowMatchingMetadataAttribute, out var expectedValidity, out var expectedResult);

        _ = new AssertionScope();
        isValid.Should().Be(expectedValidity);
        result.Should().Be(expectedResult);
    }

    protected void GeneratesTryParseTest(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute)
    {
        var isValid = TryParse(name, out var result, ignoreCase, allowMatchingMetadataAttribute);
        ValidateTryParse(name.ToString(), ignoreCase, allowMatchingMetadataAttribute, out var expectedValidity, out var expectedResult);

        _ = new AssertionScope();
        isValid.Should().Be(expectedValidity);
        result.Should().Be(expectedResult);
    }

    private void ValidateTryParse(string name, bool ignoreCase, bool allowMatchingMetadataAttribute, out bool expectedValidity, out T expectedResult)
    {
        if (allowMatchingMetadataAttribute)
        {
            expectedValidity = TryGetEnumByDisplayNameOrDescription(name, ignoreCase, out expectedResult);
            if (!expectedValidity)
            {
                expectedValidity = Enum.TryParse(name, ignoreCase, out expectedResult);
            }
        }
        else
        {
            expectedValidity = Enum.TryParse(name, ignoreCase, out expectedResult);
        }
    }

    protected void GeneratesGetValueOrDefaultTest(string name, bool ignoreCase, bool allowMatchingMetadataAttribute)
    {
        var enumValue = GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);
        ValidateTryParse(name, ignoreCase, allowMatchingMetadataAttribute, out var expectedValidity, out var expectedResult);

        _ = new AssertionScope();
        if (enumValue is null)
            expectedValidity.Should().BeFalse();
        else
        {
            expectedValidity.Should().BeTrue();
            enumValue.Should().Be(expectedResult);
        }
    }

    protected void GeneratesGetValueOrDefaultTest(in ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute)
    {
        var enumValue = GetValueOrDefault(name, ignoreCase, allowMatchingMetadataAttribute);
        ValidateTryParse(name.ToString(), ignoreCase, allowMatchingMetadataAttribute, out var expectedValidity, out var expectedResult);

        _ = new AssertionScope();
        if (enumValue is null)
            expectedValidity.Should().BeFalse();
        else
        {
            expectedValidity.Should().BeTrue();
            enumValue.Should().Be(expectedResult);
        }
    }

    protected void GeneratesGetMetadataNamesOrDefaultTest(string[] displayNames)
    {
        var expected = new List<string>(displayNames.Length);
        foreach (var value in Enum.GetValues<T>())
        {
            var valueAsString = value.ToString();
            TryGetDisplayNameOrDescription(valueAsString, out var displayName);
            expected.Add(displayName ?? valueAsString);
        }

        displayNames.Should().Equal(expected);
    }

    protected void GeneratesGetValuesTest(T[] values)
    {
        var expected = (T[])Enum.GetValues(typeof(T));
        values.Should().Equal(expected);
    }

    protected void GeneratesGetNamesTest(string[] names)
    {
        var expected = Enum.GetNames(typeof(T));
        names.Should().Equal(expected);
    }

    private bool TryGetEnumByDisplayNameOrDescription(string name, bool ignoreCase, out T enumValue)
    {
        enumValue = default;

        var stringComparisonOptions = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var enumValues = (T[])Enum.GetValues(typeof(T));
        foreach (var value in enumValues)
        {
            if (TryGetDisplayNameOrDescription(value.ToString(), out var displayName) && string.Equals(displayName, name, stringComparisonOptions))
            {
                enumValue = value;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetDisplayNameOrDescription(
        string? value,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        out string? displayName)
    {
        displayName = default;

        if (value is null) return false;
        // Prevent: Warning CS8604  Possible null reference argument for parameter 'name' in 'MemberInfo[] Type.GetMember(string name)'
        var memberInfo = typeof(T).GetMember(value);
        if (memberInfo.Length <= 0) return false;
        // Doesn't take order into account, but we don't test with both currently
        displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()?.GetName()
                      ?? memberInfo[0].GetCustomAttribute<DescriptionAttribute>()?.Description;
        return displayName is not null;
    }
}