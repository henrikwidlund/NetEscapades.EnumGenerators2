using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NetEscapades.EnumGenerators.Benchmarks;

internal static class EnumHelper
{
    internal static bool TryParseByDisplayName<T>(string name, bool ignoreCase, out T enumValue)
        where T : struct
    {
        enumValue = default;

        var stringComparisonOption = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var enumValues = (T[])Enum.GetValues(typeof(T));
        foreach (var value in enumValues)
        {
            if (TryGetDisplayName<T>(value.ToString(), out var displayName) && displayName.Equals(name, stringComparisonOption))
            {
                enumValue = value;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetDisplayName<T>(
        string? value,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        out string? displayName)
        where T : struct
    {
        displayName = default;

        if (!typeof(T).IsEnum || value is null) return false;
        // Prevent: Warning CS8604  Possible null reference argument for parameter 'name' in 'MemberInfo[] Type.GetMember(string name)'
        var memberInfo = typeof(T).GetMember(value);
        if (memberInfo.Length <= 0) return false;
        displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
        return displayName is not null;
    }

    internal static string? GetDisplayName<T>(T value)
        where T : struct
    {
        if (!typeof(T).IsEnum) return null;
        var memberInfo = typeof(T).GetMember(value.ToString()!);
        if (memberInfo.Length <= 0) return null;
        var displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()!.GetName();
        return displayName;
    }

    internal static string[] GetMetadataNamesOrDefault<T>()
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        var names = new string[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            var displayName = GetDisplayName(values[i]);
            if (displayName is not null)
            {
                names[i] = displayName;
            }
            else
            {
                names[i] = values[i].ToString();
            }
        }

        return names;
    }
}
