// <copyright file="BooleanFromStringConverter.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Reads a flag that is written in JSON either as true / false or as a "True" / "False" string.
/// </summary>
public sealed class BooleanFromStringConverter : JsonConverter<bool>
{
    /// <inheritdoc/>
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String ? bool.Parse(reader.GetString()!) : reader.GetBoolean();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
}
