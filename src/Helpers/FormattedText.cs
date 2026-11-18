// <copyright file="FormattedText.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Reads the simple markup of setting descriptions: a new line, **emphasis**, `code`, [text](url) and bare links.
/// </summary>
public static class FormattedText
{
    private const string BoldGroup = "bold";
    private const string CodeGroup = "code";
    private const string LinkTextGroup = "text";
    private const string LinkUrlGroup = "url";

    // The punctuation after a bare link is a part of the sentence, not of the address
    private static readonly char[] LinkTrailingPunctuation = ['.', ',', ';', ':', '!', '?', ')'];

    private static readonly Regex Markup = new(
        @"`(?<code>[^`]+)`|\*\*(?<bold>.+?)\*\*|\[(?<text>[^\]]+)\]\((?<url>https?://[^\s)]+)\)|https?://[^\s<>""]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

    /// <summary>
    /// Splits a text with markup into parts.
    /// </summary>
    /// <param name="text">A text with markup.</param>
    public static IReadOnlyList<FormattedTextPart> Parse(string text)
    {
        var parts = new List<FormattedTextPart>();
        var lines = text.ReplaceLineEndings("\n").Split('\n');

        for (var i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                parts.Add(new FormattedTextPart(FormattedTextPartKind.LineBreak));
            }

            ParseLine(lines[i], parts);
        }

        return parts;
    }

    /// <summary>
    /// Removes the markup from a text. It is used where the text is searched or copied.
    /// </summary>
    /// <param name="text">A text with markup.</param>
    public static string ToPlainText(string text)
    {
        var plainText = new StringBuilder();

        foreach (var part in Parse(text))
        {
            _ = part.Kind switch
            {
                FormattedTextPartKind.LineBreak => plainText.AppendLine(),
                FormattedTextPartKind.Link when part.Text != part.Url => plainText.Append(part.Text).Append(" (").Append(part.Url).Append(')'),
                _ => plainText.Append(part.Text),
            };
        }

        return plainText.ToString();
    }

    private static void ParseLine(string line, List<FormattedTextPart> parts)
    {
        var position = 0;

        foreach (Match match in Markup.Matches(line))
        {
            AddText(parts, line[position..match.Index]);
            position = match.Index + match.Length;

            if (match.Groups[CodeGroup].Success)
            {
                parts.Add(new FormattedTextPart(FormattedTextPartKind.Code, match.Groups[CodeGroup].Value));
            }
            else if (match.Groups[BoldGroup].Success)
            {
                parts.Add(new FormattedTextPart(FormattedTextPartKind.Bold, match.Groups[BoldGroup].Value));
            }
            else if (match.Groups[LinkUrlGroup].Success)
            {
                parts.Add(new FormattedTextPart(FormattedTextPartKind.Link, match.Groups[LinkTextGroup].Value, match.Groups[LinkUrlGroup].Value));
            }
            else
            {
                var url = match.Value.TrimEnd(LinkTrailingPunctuation);
                parts.Add(new FormattedTextPart(FormattedTextPartKind.Link, url, url));
                AddText(parts, match.Value[url.Length..]);
            }
        }

        AddText(parts, line[position..]);
    }

    private static void AddText(List<FormattedTextPart> parts, string text)
    {
        if (text.Length > 0)
        {
            parts.Add(new FormattedTextPart(FormattedTextPartKind.Text, text));
        }
    }
}
