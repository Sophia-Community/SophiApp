// <copyright file="ContextMenuHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using SophiApp.Extensions;
    using SophiApp.Models;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.Foundation;

    /// <summary>
    /// Helper class for working with UI element context menu.
    /// </summary>
    public static partial class ContextMenuHelper
    {
        /// <summary>
        /// Adds to the menu of a setting an item that copies the web address from the notice under the setting.
        /// The item is there only while the notice has an address: a page or a file the app could not reach.
        /// </summary>
        public static readonly DependencyProperty CopiesNoticeLinkProperty =
            DependencyProperty.RegisterAttached("CopiesNoticeLink", typeof(bool), typeof(ContextMenuHelper), new PropertyMetadata(false, OnCopiesNoticeLinkChanged));

        /// <summary>
        /// Gets a value indicating whether the menu has the item that copies the web address from the notice.
        /// </summary>
        /// <param name="menu">The menu of a setting.</param>
        public static bool GetCopiesNoticeLink(MenuFlyout menu) => (bool)menu.GetValue(CopiesNoticeLinkProperty);

        /// <summary>
        /// Turns the item that copies the web address from the notice on or off.
        /// </summary>
        /// <param name="menu">The menu of a setting.</param>
        /// <param name="value">True to have the item.</param>
        public static void SetCopiesNoticeLink(MenuFlyout menu, bool value) => menu.SetValue(CopiesNoticeLinkProperty, value);

        /// <summary>
        /// Copy <paramref name="text"/> to clipboard.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void CopyToClipboard(string? text)
        {
            var dataPackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        /// <summary>
        /// Opens the specified <paramref name="path"/> with Windows Explorer.
        /// </summary>
        /// <param name="path">Path to open in Windows Explorer.</param>
        public static void OpenInExplorer(string path) => ExplorerHelper.Open(path);

        /// <summary>
        /// Shows the flyout placed in relation to the specified element using the specified options.
        /// </summary>
        /// <param name="sender">UIElement is a base class for most of the Windows Runtime UI objects.</param>
        /// <param name="flyout">Flyout displayed on UI element.</param>
        /// <param name="args">Provides event data for the ContextRequested event.</param>
        public static void ShowContextMenu(UIElement sender, FlyoutBase flyout, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
        {
            args.Handled = true;
            Point point;
            _ = args.TryGetPosition(sender, out point);

            // The menu opens to the right of the pointer and below it, like the menus of Windows
            var options = new FlyoutShowOptions()
            {
                Position = point,
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = FlyoutShowMode.Standard,
            };
            PopupCursor.SetResetOnOpen(flyout, true);
            flyout.ShowAt((FrameworkElement)sender, options);
        }

        private static void OnCopiesNoticeLinkChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not MenuFlyout menu || !(bool)args.NewValue)
            {
                return;
            }

            MenuFlyoutItem? item = null;

            menu.Opening += (_, _) =>
            {
                // The item is made when the menu opens for the first time, so it stands after the items of the markup
                if (item is null)
                {
                    item = new MenuFlyoutItem { Text = "UIModel_ContextMenu_CopyLink".GetLocalized(), Icon = new SymbolIcon(Symbol.Link) };
                    item.Click += (_, _) => CopyToClipboard(GetNoticeLink(menu));
                    menu.Items.Add(item);
                }

                item.Visibility = GetNoticeLink(menu).Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            };
        }

        // The first web address in the notice of the setting the menu is opened for, without the punctuation of the sentence
        private static string GetNoticeLink(MenuFlyout menu)
            => (menu.Target?.DataContext as UIModel)?.ExceptionText is { Length: > 0 } notice
                ? WebAddress().Match(notice).Value.TrimEnd('.', ',', ';', ':', ')', '"')
                : string.Empty;

        [GeneratedRegex(@"https?://\S+")]
        private static partial Regex WebAddress();
    }
}
