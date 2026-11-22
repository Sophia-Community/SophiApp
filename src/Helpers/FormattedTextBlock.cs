// <copyright file="FormattedTextBlock.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using System.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SophiApp.Contracts.Services;

/// <summary>
/// Shows a text with the markup of <see cref="FormattedText"/> in a text block.
/// </summary>
internal static class FormattedTextBlock
{
    // Invisible, forbids a line break where it stands
    private const char WordJoiner = '⁠';
    private const char NoBreakSpace = ' ';

    // Both fonts come with Windows, the second one is used if the first is removed
    private static readonly FontFamily CodeFont = new("Cascadia Mono, Consolas");

    // The text blocks whose emphasized words already follow the theme
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TextBlock, object> Followed = [];

    /// <summary>
    /// Replaces the content of a text block with a formatted text.
    /// </summary>
    /// <param name="block">The text block to fill.</param>
    /// <param name="text">A text with markup.</param>
    public static void SetText(TextBlock block, string text)
    {
        block.Inlines.Clear();

        foreach (var part in FormattedText.Parse(text))
        {
            block.Inlines.Add(part.Kind switch
            {
                FormattedTextPartKind.Bold => new Run { Text = KeepShortcutsOnOneLine(part.Text), FontWeight = FontWeights.SemiBold, Foreground = GetEmphasisBrush(block) },
                FormattedTextPartKind.Code => new Run { Text = KeepOnOneLine(part.Text), FontFamily = CodeFont },
                FormattedTextPartKind.Link => CreateLink(part),
                FormattedTextPartKind.LineBreak => new LineBreak(),
                _ => new Run { Text = KeepShortcutsOnOneLine(part.Text) },
            });
        }

        // A run made in code does not follow the theme by itself
        if (!Followed.TryGetValue(block, out _))
        {
            Followed.Add(block, block);
            block.Loaded += (_, _) => ColorEmphasis(block);
            block.ActualThemeChanged += (_, _) => ColorEmphasis(block);
        }
    }

    /// <summary>
    /// Makes key combinations such as "Win+Shift+S" stay on one line: a line can be broken before a plus sign.
    /// The text itself is not changed for the search and the clipboard: only what is shown gets the invisible joiners.
    /// </summary>
    /// <param name="text">A text as it is.</param>
    /// <returns>The text to show.</returns>
    internal static string KeepShortcutsOnOneLine(string text)
    {
        if (!text.Contains('+'))
        {
            return text;
        }

        var result = new StringBuilder(text.Length + 8);

        for (var i = 0; i < text.Length; i++)
        {
            // A plus sign right after a letter, a digit or another plus sign is a part of a key combination or of a name like "C++"
            var joins = text[i] == '+' && i > 0 && (char.IsLetterOrDigit(text[i - 1]) || text[i - 1] == '+');

            if (joins)
            {
                result.Append(WordJoiner);
            }

            result.Append(text[i]);

            if (joins)
            {
                result.Append(WordJoiner);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Makes a path, a command or a link stay on one line: a line can be broken after a space, a percent sign or a backslash,
    /// and a path or a link cut in two is hard to read.
    /// </summary>
    private static string KeepOnOneLine(string text)
    {
        var result = new StringBuilder(text.Length * 2);

        for (var i = 0; i < text.Length; i++)
        {
            var symbol = text[i] == ' ' ? NoBreakSpace : text[i];

            // Letters and digits are not parted anyway, and a screen reader gets whole words
            if (i > 0 && !(char.IsLetterOrDigit(text[i - 1]) && char.IsLetterOrDigit(text[i])))
            {
                result.Append(WordJoiner);
            }

            result.Append(symbol);
        }

        return result.ToString();
    }

    // Emphasized words have the color of the accent text of Windows: darker than the accent in the light theme, lighter in the dark one.
    // In a high contrast theme they keep the color of the text
    private static Brush? GetEmphasisBrush(TextBlock block)
    {
        if (new Windows.UI.ViewManagement.AccessibilitySettings().HighContrast)
        {
            return block.Foreground;
        }

        var key = block.ActualTheme == ElementTheme.Light ? "SystemAccentColorDark2" : "SystemAccentColorLight3";
        return Application.Current.Resources.TryGetValue(key, out var color) && color is Windows.UI.Color accent ? new SolidColorBrush(accent) : block.Foreground;
    }

    private static void ColorEmphasis(TextBlock block)
    {
        foreach (var run in block.Inlines.OfType<Run>().Where(run => run.FontWeight.Weight == FontWeights.SemiBold.Weight))
        {
            run.Foreground = GetEmphasisBrush(block);
        }
    }

    private static Hyperlink CreateLink(FormattedTextPart part)
    {
        var link = new Hyperlink();
        link.Inlines.Add(new Run { Text = KeepOnOneLine(part.Text) });

        // The address is opened by the app itself: it runs elevated, and the browser must not
        link.Click += (_, _) => App.GetService<IHttpService>().OpenUrl(part.Url);
        return link;
    }
}
