# NetEscapades.EnumGenerators

![Build status](https://github.com/andrewlock/NetEscapades.EnumGenerators/actions/workflows/BuildAndPack.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/NetEscapades.EnumGenerators.svg)](https://www.nuget.org/packages/NetEscapades.EnumGenerators/)

A Source Generator package that generates extension methods for enums, to allow fast "reflection".

> This source generator requires the .NET 7 SDK.

Add the package to your application using

```bash
dotnet add package NetEscapades.EnumGenerators
```


This adds a `<PackageReference>` to your project. You can additionally mark the package as `PrivateAssets="all"` and `ExcludeAssets="runtime"`.

> Setting `PrivateAssets="all"` means any projects referencing this one won't get a reference to the _NetEscapades.EnumGenerators_ package. Setting `ExcludeAssets="runtime"` ensures the _NetEscapades.EnumGenerators.Attributes.dll_ file is not copied to your build output (it is not required at runtime).

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="NetEscapades.EnumGenerators" Version="1.0.0-beta04" 
    PrivateAssets="all" ExcludeAssets="runtime" />
  <!-- -->

</Project>
```

Adding the package will automatically add a marker attribute, `[EnumExtensions]`, to your project.

To use the generator, add the `[EnumExtensions]` attribute to an enum. For example:

```csharp
using System.ComponentModel.DataAnnotations;

[EnumExtensions]
public enum MyEnum
{
    First,
    [Display(Name = "2nd")] Second
}
```

This will generate a class called `MyEnumExtensions` (by default), which contains a number of helper methods. For example:

```csharp
/// <summary>
/// Extension methods for <see cref="global::MyEnum" />.
/// </summary>
public static partial class MyEnumExtensions
{
    /// <summary>
    /// The number of members in the enum.
    /// This is a non-distinct count of defined names.
    /// </summary>
    public const int Length = 2;

    /// <summary>
    /// Returns the string representation of the <see cref="global::MyEnum"/> value.
    /// If the attribute is decorated with a <c>[Display]</c>/<c>[Description]</c> attribute, then
    /// uses the provided value. Otherwise uses the name of the member, equivalent to
    /// calling <c>ToString()</c> on <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to retrieve the string value for.</param>
    /// <returns>The string representation of the value.</returns>
    public static string ToStringFast(this global::MyEnum value)
        => value switch
        {
            global::MyEnum.First => nameof(global::MyEnum.First),
            global::MyEnum.Second => "2nd",
            _ => value.ToString()
        };

    /// <summary>
    /// Returns a boolean telling whether the given enum value exists in the enumeration.
    /// </summary>
    /// <param name="value">The value to check if it's defined.</param>
    /// <returns><see langword="true" /> if the value exists in the enumeration, <see langword="false" /> otherwise.</returns>
    public static bool IsDefined(global::MyEnum value)
        => value switch
        {
            global::MyEnum.First => true,
            global::MyEnum.Second => true,
            _ => false
        };

    /// <summary>
    /// Returns a boolean telling whether an enum with the given name exists in the enumeration.
    /// </summary>
    /// <param name="name">The name to check if it's defined.</param>
    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, <see langword="false" /> otherwise.</returns>
    public static bool IsDefined(string name) => IsDefined(name, false);

    /// <summary>
    /// Returns a boolean telling whether an enum with the given name exists in the enumeration,
    /// or if a member decorated with a <c>[Display]</c>/<c>[Description]</c> attribute
    /// with the required name exists.
    /// </summary>
    /// <param name="name">The name to check if it's defined.</param>
    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value of metadata attributes, otherwise ignores them.</param>
    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, or a member is decorated
    /// with a <c>[Display]</c>/<c>[Description]</c> attribute with the name, <see langword="false" /> otherwise.</returns>
    public static bool IsDefined(string name, bool allowMatchingMetadataAttribute)
    {
        var isDefinedInDisplayAttribute = false;
        if (allowMatchingMetadataAttribute)
        {
            isDefinedInDisplayAttribute = name switch
            {
                "2nd" => true,
                _ => false
            };
        }

        if (isDefinedInDisplayAttribute)
        {
            return true;
        }

        return name switch
        {
            nameof(global::MyEnum.First) => true,
            nameof(global::MyEnum.Second) => true,
            _ => false
        };
    }

    /// <summary>
    /// Returns a boolean telling whether an enum with the given name exists in the enumeration.
    /// </summary>
    /// <param name="name">The name to check if it's defined.</param>
    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, <see langword="false" /> otherwise.</returns>
    public static bool IsDefined(in global::System.ReadOnlySpan<char> name) => IsDefined(name, false);

    /// <summary>
    /// Returns a boolean telling whether an enum with the given name exists in the enumeration,
    /// or optionally if a member decorated with a <c>[Display]</c>/<c>[Description]</c> attribute
    /// with the required name exists.
    /// </summary>
    /// <param name="name">The name to check if it's defined.</param>
    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value of metadata attributes, otherwise ignores them.</param>
    /// <returns><see langword="true" /> if a member with the name exists in the enumeration, or a member is decorated
    /// with a <c>[Display]</c>/<c>[Description]</c> attribute with the name, <see langword="false" /> otherwise.</returns>
    public static bool IsDefined(in global::System.ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
    {
        var isDefinedInDisplayAttribute = false;
        if (allowMatchingMetadataAttribute)
        {
            isDefinedInDisplayAttribute = name switch
            {
                var current when global::System.MemoryExtensions.Equals(current, "2nd".AsSpan(), global::System.StringComparison.Ordinal) => true,
                _ => false
            };
        }

        if (isDefinedInDisplayAttribute)
        {
            return true;
        }

        return name switch
        {
            var current when global::System.MemoryExtensions.Equals(current, "First".AsSpan(), global::System.StringComparison.Ordinal) => true,
            var current when global::System.MemoryExtensions.Equals(current, "Second".AsSpan(), global::System.StringComparison.Ordinal) => true,
            _ => false
        };
    }

    /// <summary>
    /// Converts the string representation of the name or numeric value of
    /// an <see cref="global::MyEnum" /> to the equivalent instance.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="name">The case-sensitive string representation of the enumeration name or underlying value to convert.</param>
    /// <param name="value">When this method returns, contains an object of type
    /// <see cref="global::MyEnum" /> whose
    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
    /// If the parse operation fails, contains the default value of the underlying type
    /// of <see cref="global::MyEnum" />. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(
        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        string? name,
        out global::MyEnum value)
        => TryParse(name, out value, false, false);

    /// <summary>
    /// Converts the string representation of the name or numeric value of
    /// an <see cref="global::MyEnum" /> to the equivalent instance.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="name">The string representation of the enumeration name or underlying value to convert.</param>
    /// <param name="value">When this method returns, contains an object of type
    /// <see cref="global::MyEnum" /> whose
    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
    /// If the parse operation fails, contains the default value of the underlying type
    /// of <see cref="global::MyEnum" />. This parameter is passed uninitialized.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(
        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        string? name,
        out global::MyEnum value,
        bool ignoreCase)
        => TryParse(name, out value, ignoreCase, false);

    /// <summary>
    /// Converts the string representation of the name or numeric value of
    /// an <see cref="global::MyEnum" /> to the equivalent instance.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="name">The string representation of the enumeration name or underlying value to convert.</param>
    /// <param name="value">When this method returns, contains an object of type
    /// <see cref="global::MyEnum" /> whose
    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
    /// If the parse operation fails, contains the default value of the underlying type
    /// of <see cref="global::MyEnum" />. This parameter is passed uninitialized.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value included in metadata attributes such as
    /// <c>[Display]</c>/<c>[Description]</c> attribute when parsing, otherwise only considers the member names.</param>
    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(
        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        string? name,
        out global::MyEnum value,
        bool ignoreCase,
        bool allowMatchingMetadataAttribute)
    {
        if (allowMatchingMetadataAttribute)
        {
            if (ignoreCase)
            {
                switch (name)
                {
                    case not null when name.Equals("2nd", global::System.StringComparison.OrdinalIgnoreCase):
                        value = global::MyEnum.Second;
                        return true;
                }
            }
            else
            {
                switch (name)
                {
                    case "2nd":
                        value = global::MyEnum.Second;
                        return true;
                }
            }
        }

        if (ignoreCase)
        {
            switch (name)
            {
                case not null when name.Equals(nameof(global::MyEnum.First), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::MyEnum.First;
                    return true;
                case not null when name.Equals(nameof(global::MyEnum.Second), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::MyEnum.Second;
                    return true;
                case { Length: > 0 } when int.TryParse(name, out var val):
                    value = (global::MyEnum)val;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        switch (name)
        {
            case nameof(global::MyEnum.First):
                value = global::MyEnum.First;
                return true;
            case nameof(global::MyEnum.Second):
                value = global::MyEnum.Second;
                return true;
            case { Length: > 0 } when int.TryParse(name, out var val):
                value = (global::MyEnum)val;
                return true;
            default:
                value = default;
                return false;
        }
    }

    /// <summary>
    /// Converts the span representation of the name or numeric value of
    /// an <see cref="global::MyEnum" /> to the equivalent instance.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="name">The span representation of the enumeration name or underlying value to convert.</param>
    /// <param name="value">When this method returns, contains an object of type
    /// <see cref="global::MyEnum" /> whose
    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
    /// If the parse operation fails, contains the default value of the underlying type
    /// of <see cref="global::MyEnum" />. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(
        in global::System.ReadOnlySpan<char> name,
        out global::MyEnum value)
        => TryParse(name, out value, false, false);

    /// <summary>
    /// Converts the span representation of the name or numeric value of
    /// an <see cref="global::MyEnum" /> to the equivalent instance.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="name">The span representation of the enumeration name or underlying value to convert.</param>
    /// <param name="value">When this method returns, contains an object of type
    /// <see cref="global::MyEnum" /> whose
    /// value is represented by <paramref name="value"/> if the parse operation succeeds.
    /// If the parse operation fails, contains the default value of the underlying type
    /// of <see cref="global::MyEnum" />. This parameter is passed uninitialized.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(
        in global::System.ReadOnlySpan<char> name,
        out global::MyEnum value,
        bool ignoreCase)
        => TryParse(name, out value, ignoreCase, false);

    /// <summary>
    /// Converts the span representation of the name or numeric value of
    /// an <see cref="global::MyEnum" /> to the equivalent instance.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="name">The span representation of the enumeration name or underlying value to convert.</param>
    /// <param name="result">When this method returns, contains an object of type
    /// <see cref="global::MyEnum" /> whose
    /// value is represented by <paramref name="result"/> if the parse operation succeeds.
    /// If the parse operation fails, contains the default value of the underlying type
    /// of <see cref="global::MyEnum" />. This parameter is passed uninitialized.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode; <see langword="false" /> to read value in case sensitive mode.</param>
    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />, considers the value included in metadata attributes such as
    /// <c>[Display]</c>/<c>[Description]</c> attribute when parsing, otherwise only considers the member names.</param>
    /// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(
        in global::System.ReadOnlySpan<char> name,
        out global::MyEnum result,
        bool ignoreCase,
        bool allowMatchingMetadataAttribute)
    {
        if (allowMatchingMetadataAttribute)
        {
            if (ignoreCase)
            {
                switch (name)
                {
                    case var current when global::System.MemoryExtensions.Equals(current, "2nd".AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                        result = global::MyEnum.Second;
                        return true;
                }
            }
            else
            {
                switch (name)
                {
                    case var current when global::System.MemoryExtensions.Equals(current, "2nd".AsSpan(), global::System.StringComparison.Ordinal):
                        result = global::MyEnum.Second;
                        return true;
                }
            }
        }

        if (ignoreCase)
        {
            switch (name)
            {
                case var current when global::System.MemoryExtensions.Equals(current, "First".AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::MyEnum.First;
                    return true;
                case var current when global::System.MemoryExtensions.Equals(current, "Second".AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::MyEnum.Second;
                    return true;
                case { IsEmpty: false } when int.TryParse(name, out var numericResult):
                    result = (global::MyEnum)numericResult;
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        switch (name)
        {
            case var current when global::System.MemoryExtensions.Equals(current, "First".AsSpan(), global::System.StringComparison.Ordinal):
                result = global::MyEnum.First;
                return true;
            case var current when global::System.MemoryExtensions.Equals(current, "Second".AsSpan(), global::System.StringComparison.Ordinal):
                result = global::MyEnum.Second;
                return true;
            case { IsEmpty: false } when int.TryParse(name, out var numericResult):
                result = (global::MyEnum)numericResult;
                return true;
            default:
                result = default;
                return false;
        }
    }

    /// <summary>
    /// Retrieves an array of the metadata or <see langword="default" /> values of the members defined in
    /// <see cref="global::MyEnum" />.
    /// Note that this returns a new array with every invocation, so
    /// should be cached if appropriate.
    /// </summary>
    /// <returns>An array of the metadata or <see langword="default" /> values defined in <see cref="global::MyEnum" />.</returns>
    public static string[] GetMetadataNamesOrDefault() =>
        new[]
        {
            nameof(global::MyEnum.First),
            "2nd",
        };

    /// <summary>
    /// Gets the <see cref="global::MyEnum" /> representation of the <paramref name="name"/>
    /// or <see langword="default" /> if there's no match.
    /// </summary>
    /// <param name="name">The value that should be matched.</param>
    /// <returns>The matching <see cref="global::MyEnum" /> or <see langword="null" /> if there was no match.</returns>
    public static global::MyEnum? GetValueOrDefault(string? name) =>
        TryParse(name, out global::MyEnum value) ? value : null;

    /// <summary>
    /// Gets the <see cref="global::MyEnum" /> representation of the <paramref name="name"/>
    /// or <see langword="default" /> if there's no match.
    /// </summary>
    /// <param name="name">The value that should be matched.</param>
    /// <returns>The matching <see cref="global::MyEnum" /> or <see langword="null" /> if there was no match.</returns>
    public static global::MyEnum? GetValueOrDefault(in global::System.ReadOnlySpan<char> name) =>
        TryParse(name, out global::MyEnum value) ? value : null;

    /// <summary>
    /// Gets the <see cref="global::MyEnum" /> representation of the <paramref name="name"/>
    /// or <see langword="default" /> if there's no match.
    /// </summary>
    /// <param name="name">The value that should be matched.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
    /// <see langword="false" /> to read value in case sensitive mode.</param>
    /// <returns>The matching <see cref="global::MyEnum" /> or <see langword="null" /> if there was no match.</returns>
    public static global::MyEnum? GetValueOrDefault(string? name, bool ignoreCase) =>
        TryParse(name, out global::MyEnum value, ignoreCase) ? value : null;

    /// <summary>
    /// Gets the <see cref="global::MyEnum" /> representation of the <paramref name="name"/>
    /// or <see langword="default" /> if there's no match.
    /// </summary>
    /// <param name="name">The value that should be matched.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
    /// <see langword="false" /> to read value in case sensitive mode.</param>
    /// <returns>The matching <see cref="global::MyEnum" /> or <see langword="null" /> if there was no match.</returns>
    public static global::MyEnum? GetValueOrDefault(in global::System.ReadOnlySpan<char> name, bool ignoreCase) =>
        TryParse(name, out global::MyEnum value, ignoreCase) ? value : null;

    /// <summary>
    /// Gets the <see cref="global::MyEnum" /> representation of the <paramref name="name"/>
    /// or <see langword="default" /> if there's no match.
    /// </summary>
    /// <param name="name">The value that should be matched.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
    /// <see langword="false" /> to read value in case sensitive mode.</param>
    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />,
    /// considers the value of metadata attributes, otherwise ignores them.</param>
    /// <returns>The matching <see cref="global::MyEnum" /> or <see langword="null" /> if there was no match.</returns>
    public static global::MyEnum? GetValueOrDefault(string? name, bool ignoreCase, bool allowMatchingMetadataAttribute) =>
        TryParse(name, out global::MyEnum value, ignoreCase, allowMatchingMetadataAttribute) ? value : null;

    /// <summary>
    /// Gets the <see cref="global::MyEnum" /> representation of the <paramref name="name"/>
    /// or <see langword="default" /> if there's no match.
    /// </summary>
    /// <param name="name">The value that should be matched.</param>
    /// <param name="ignoreCase"><see langword="true" /> to read value in case insensitive mode;
    /// <see langword="false" /> to read value in case sensitive mode.</param>
    /// <param name="allowMatchingMetadataAttribute">If <see langword="true" />,
    /// considers the value of metadata attributes, otherwise ignores them.</param>
    /// <returns>The matching <see cref="global::MyEnum" /> or <see langword="null" /> if there was no match.</returns>
    public static global::MyEnum? GetValueOrDefault(in global::System.ReadOnlySpan<char> name, bool ignoreCase, bool allowMatchingMetadataAttribute) =>
        TryParse(name, out global::MyEnum value, ignoreCase, allowMatchingMetadataAttribute) ? value : null;

    /// <summary>
    /// Retrieves an array of the values of the members defined in
    /// <see cref="global::MyEnum" />.
    /// Note that this returns a new array with every invocation, so
    /// should be cached if appropriate.
    /// </summary>
    /// <returns>An array of the values defined in <see cref="global::MyEnum" />.</returns>
    public static global::MyEnum[] GetValues() =>
        new[]
        {
            global::MyEnum.First,
            global::MyEnum.Second,
        };

    /// <summary>
    /// Retrieves an array of the names of the members defined in
    /// <see cref="global::MyEnum" />.
    /// Note that this returns a new array with every invocation, so
    /// should be cached if appropriate.
    /// </summary>
    /// <returns>An array of the names of the members defined in <see cref="global::MyEnum" />.</returns>
    public static string[] GetNames() =>
        new[]
        {
            nameof(global::MyEnum.First),
            nameof(global::MyEnum.Second),
        };
}
```

You can override the name of the extension class by setting `ExtensionClassName` in the attribute and/or the namespace of the class by setting `ExtensionClassNamespace`. By default, the class will be public if the enum is public, otherwise it will be internal.

If you want a `JsonConverter` that uses the generated extensions for efficient serialization and deserialization you can add the `EnumJsonConverter` and `JsonConverter` to the enum. For example:
```csharp
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

[EnumExtensions]
[EnumJsonConverter(typeof(MyEnumConverter))]
[JsonConverter(typeof(MyEnumConverter))]
public enum MyEnum
{
    First,
    [Display(Name = "2nd")] Second
}
```

This will generate a class called `MyEnumConverter`. For example:
```csharp
/// <summary>
/// Converts a <see cref="global::MyEnum" /> to or from JSON.
/// </summary>
public sealed class MyEnumConverter : global::System.Text.Json.Serialization.JsonConverter<MyEnum>
{
    /// <inheritdoc />
    /// <summary>
    /// Read and convert the JSON to <see cref="global::MyEnum" />.
    /// </summary>
    /// <remarks>
    /// A converter may throw any Exception, but should throw <see cref="global::System.Text.Json.JsonException" /> when the JSON is invalid.
    /// </remarks>
    public override global::MyEnum Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
    {
        char[]? rentedBuffer = null;
        var bufferLength = reader.HasValueSequence ? checked((int)reader.ValueSequence.Length) : reader.ValueSpan.Length;

        var charBuffer = bufferLength <= 128
            ? stackalloc char[128]
            : rentedBuffer = global::System.Buffers.ArrayPool<char>.Shared.Rent(bufferLength);

        var charsWritten = reader.CopyString(charBuffer);
        global::System.ReadOnlySpan<char> source = charBuffer[..charsWritten];
        try
        {
            if (global::MyEnumExtensions.TryParse(source, out var enumValue, true, false))
                return enumValue;

            throw new global::System.Text.Json.JsonException($"{source.ToString()} is not a valid value.", null, null, null);
        }
        finally
        {
            if (rentedBuffer is not null)
            {
                charBuffer[..charsWritten].Clear();
                global::System.Buffers.ArrayPool<char>.Shared.Return(rentedBuffer);
            }
        }
    }

    /// <inheritdoc />
    public override void Write(global::System.Text.Json.Utf8JsonWriter writer, global::MyEnum value, global::System.Text.Json.JsonSerializerOptions options)
        => writer.WriteStringValue(global::MyEnumExtensions.ToStringFast(value));
}
```

You can customize the generated code for the converter by setting the following values:
- `CaseSensitive` - Indicates if the string representation is case sensitive when deserializing it as an enum.
- `CamelCase` - Indicates if the value of `PropertyName` should be camel cased.
- `AllowMatchingMetadataAttribute` - If `true`, considers the value of metadata attributes, otherwise ignores them.
- `PropertyName` - If set, this value will be used in messages when there are problems with validation and/or serialization/deserialization occurs.
And a class called `MyEnumJsonConverter` that uses the extensions generated by the `EnumExtensions` attribute. For example:

## Embedding the attributes in your project

By default, the `[EnumExtensions]` and `[EnumJsonConverter]` attributes referenced in your application are contained in an external dll. It is also possible to embed the attributes directly in your project, so they appear in the dll when your project is built. If you wish to do this, you must do two things:

1. Define the MSBuild constant `NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES`. This ensures the attributes are embedded in your project
2. Add `compile` to the list of excluded assets in your `<PackageReference>` element. This ensures the attributes in your project are referenced, instead of the _NetEscapades.EnumGenerators.Attributes.dll_ library.

Your project file should look something like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <!--  Define the MSBuild constant    -->
    <DefineConstants>NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES</DefineConstants>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="NetEscapades.EnumGenerators" Version="1.0.0-beta04" 
                    PrivateAssets="all"
                    ExcludeAssets="compile;runtime" />
<!--                               ☝ Add compile to the list of excluded assets. -->

</Project>
```

## Preserving usages of the `[EnumExtensions]` and `[EnumJsonConverter]` attributes

The `[EnumExtensions]` and `[EnumJsonConverter]` attributes are decorated with the `[Conditional]` attribute, [so their usage will not appear in the build output of your project](https://andrewlock.net/conditional-compilation-for-ignoring-method-calls-with-the-conditionalattribute/#applying-the-conditional-attribute-to-classes). If you use reflection at runtime on one of your `enum`s, you will not find `[EnumExtensions]` or `[EnumJsonConverter]` in the list of custom attributes.

If you wish to preserve these attributes in the build output, you can define the `NETESCAPADES_ENUMGENERATORS_USAGES` MSBuild variable. Note that this means your project will have a runtime-dependency on _NetEscapades.EnumGenerators.Attributes.dll_ so you need to ensure this is included in your build output.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <!--  Define the MSBuild constant to preserve usages   -->
    <DefineConstants>NETESCAPADES_ENUMGENERATORS_USAGES</DefineConstants>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="NetEscapades.EnumGenerators" Version="1.0.0-beta05" PrivateAssets="all" />
  <!--              ☝ You must not exclude the runtime assets in this case -->

</Project>
```

## Error CS0436 and [InternalsVisibleTo]

> In the latest version of _NetEscapades.EnumGenerators_, you should not experience error CS0436 by default.

In previous versions of the _NetEscapades.EnumGenerators_ generator, the `[EnumExtensions]` attributes were added to your compilation as `internal` attributes by default. If you added the source generator package to multiple projects, and used the `[InternalsVisibleTo]` attribute, you could experience errors when you build:

```bash
warning CS0436: The type 'EnumExtensionsAttribute' in 'NetEscapades.EnumGenerators\NetEscapades.EnumGenerators\EnumExtensionsAttribute.cs' conflicts with the imported type 'EnumExtensionsAttribute' in 'MyProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.
```

In the latest version of _StronglyTypedId_, the attributes are not embedded by default, so you should not experience this problem. If you see this error, compare your installation to the examples in the installation guide.
