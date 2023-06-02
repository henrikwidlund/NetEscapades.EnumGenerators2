using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NetEscapades.EnumGenerators.Benchmarks;

internal static class EnumHelper<T> where T : struct
{
    internal static bool TryParseByDisplayName(string name, bool ignoreCase, out T enumValue)
    {
        enumValue = default;

        var stringComparisonOption = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var enumValues = (T[])Enum.GetValues(typeof(T));
        foreach (var value in enumValues)
        {
            if (TryGetDisplayName(value.ToString(), out var displayName) && displayName.Equals(name, stringComparisonOption))
            {
                enumValue = value;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetDisplayName(
        string? value,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        out string? displayName)
    {
        displayName = default;

        if (!typeof(T).IsEnum || value is null) return false;
        // Prevent: Warning CS8604  Possible null reference argument for parameter 'name' in 'MemberInfo[] Type.GetMember(string name)'
        var memberInfo = typeof(T).GetMember(value);
        if (memberInfo.Length <= 0) return false;
        displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
        return displayName is not null;
    }

    internal static string GetDisplayName(T value)
    {
        if (!typeof(T).IsEnum) return string.Empty;
        var memberInfo = typeof(T).GetMember(value.ToString()!);
        if (memberInfo.Length <= 0) return string.Empty;
        var displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()!.GetName();
        return displayName ?? string.Empty;
    }
}
