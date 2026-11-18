// <copyright file="InfoBarHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

/// <summary>
/// A helper for work with <see cref="InfoBar"/>.
/// </summary>
public static class InfoBarHelper
{
    /// <summary>
    /// Turns on the markup of <see cref="FormattedText"/> in the message of an info bar.
    /// </summary>
    public static readonly DependencyProperty FormattedMessageProperty =
        DependencyProperty.RegisterAttached("FormattedMessage", typeof(bool), typeof(InfoBarHelper), new PropertyMetadata(false, OnFormattedMessageChanged));

    // The name of the message text block in the template of the info bar
    private const string MessageBlockName = "Message";

    // The message as it was written, with the markup
    private static readonly DependencyProperty MarkupProperty =
        DependencyProperty.RegisterAttached("Markup", typeof(string), typeof(InfoBarHelper), new PropertyMetadata(null));

    /// <summary>
    /// Gets a value indicating whether the markup is turned on in the message of an info bar.
    /// </summary>
    /// <param name="infoBar">An info bar.</param>
    public static bool GetFormattedMessage(InfoBar infoBar) => (bool)infoBar.GetValue(FormattedMessageProperty);

    /// <summary>
    /// Turns the markup in the message of an info bar on or off.
    /// </summary>
    /// <param name="infoBar">An info bar.</param>
    /// <param name="value">True to turn the markup on.</param>
    public static void SetFormattedMessage(InfoBar infoBar, bool value) => infoBar.SetValue(FormattedMessageProperty, value);

    private static void OnFormattedMessageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is InfoBar infoBar)
        {
            infoBar.Loaded -= OnInfoBarLoaded;

            if ((bool)args.NewValue)
            {
                infoBar.Loaded += OnInfoBarLoaded;
            }
        }
    }

    private static void OnInfoBarLoaded(object sender, RoutedEventArgs args)
    {
        var infoBar = (InfoBar)sender;
        var markup = infoBar.GetValue(MarkupProperty) as string ?? infoBar.Message;
        infoBar.SetValue(MarkupProperty, markup);

        // Screen readers get the message without the markup
        infoBar.Message = FormattedText.ToPlainText(markup);

        if (FindMessageBlock(infoBar) is TextBlock block)
        {
            FormattedTextBlock.SetText(block, markup);
        }
    }

    private static TextBlock? FindMessageBlock(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is TextBlock { Name: MessageBlockName } block)
            {
                return block;
            }

            if (FindMessageBlock(child) is TextBlock found)
            {
                return found;
            }
        }

        return null;
    }
}
