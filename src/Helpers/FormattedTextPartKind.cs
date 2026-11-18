// <copyright file="FormattedTextPartKind.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

/// <summary>
/// The kinds of parts a formatted text consists of.
/// </summary>
public enum FormattedTextPartKind
{
    /// <summary>
    /// A plain text.
    /// </summary>
    Text,

    /// <summary>
    /// An emphasized text, written as **text**.
    /// </summary>
    Bold,

    /// <summary>
    /// A name of a file, a command or a registry value, written as `text`.
    /// </summary>
    Code,

    /// <summary>
    /// A link, written as [text](url) or as a bare url.
    /// </summary>
    Link,

    /// <summary>
    /// A line break, written as a new line.
    /// </summary>
    LineBreak,
}
