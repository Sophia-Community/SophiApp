// <copyright file="Json.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using SophiApp.Helpers;

/// <summary>
/// Json extensions.
/// </summary>
public static class Json
{
    // The files the app reads are written by hand and by other programs: names differ in case,
    // numbers and flags are written as strings, there are comments and trailing commas
    private static readonly JsonSerializerOptions Options = new()
    {
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        Converters = { new BooleanFromStringConverter() },
    };

    /// <summary>
    /// Deserializes the JSON to the specified .NET type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="value">The JSON to deserialize.</param>
    public static T ToObject<T>(string value) => JsonSerializer.Deserialize<T>(value, Options)!;

    /// <summary>
    /// Deserializes the JSON to the specified .NET type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="value">The JSON to deserialize.</param>
    public static async Task<T> ToObjectAsync<T>(string value) => await Task.Run(() => ToObject<T>(value));

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    public static string Stringify(object value) => JsonSerializer.Serialize(value, Options);

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    public static async Task<string> StringifyAsync(object value) => await Task.Run(() => Stringify(value));
}
