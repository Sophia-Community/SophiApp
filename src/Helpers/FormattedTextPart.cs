// <copyright file="FormattedTextPart.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

/// <summary>
/// A part of a formatted text.
/// </summary>
public sealed class FormattedTextPart
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormattedTextPart"/> class.
    /// </summary>
    /// <param name="kind">The kind of the part.</param>
    /// <param name="text">The text to show.</param>
    /// <param name="url">The link address, empty for other kinds.</param>
    public FormattedTextPart(FormattedTextPartKind kind, string text = "", string url = "")
    {
        Kind = kind;
        Text = text;
        Url = url;
    }

    /// <summary>
    /// Gets the kind of the part.
    /// </summary>
    public FormattedTextPartKind Kind { get; }

    /// <summary>
    /// Gets the text to show.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the link address, empty for other kinds.
    /// </summary>
    public string Url { get; }
}
