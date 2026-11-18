// <copyright file="FormattedTextBlock.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SophiApp.Contracts.Services;

/// <summary>
/// Shows a text with the markup of <see cref="FormattedText"/> in a text block.
/// </summary>
internal static class FormattedTextBlock
{
    // Both fonts come with Windows, the second one is used if the first is removed
    private static readonly FontFamily CodeFont = new("Cascadia Mono, Consolas");

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
                FormattedTextPartKind.Bold => new Run { Text = part.Text, FontWeight = FontWeights.SemiBold },
                FormattedTextPartKind.Code => new Run { Text = part.Text, FontFamily = CodeFont },
                FormattedTextPartKind.Link => CreateLink(part),
                FormattedTextPartKind.LineBreak => new LineBreak(),
                _ => new Run { Text = part.Text },
            });
        }
    }

    private static Hyperlink CreateLink(FormattedTextPart part)
    {
        var link = new Hyperlink();
        link.Inlines.Add(new Run { Text = part.Text });

        // The address is opened by the app itself: it runs elevated, and the browser must not
        link.Click += (_, _) => App.GetService<IHttpService>().OpenUrl(part.Url);
        return link;
    }
}
