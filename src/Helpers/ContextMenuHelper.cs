// <copyright file="ContextMenuHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using System.Diagnostics;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.Foundation;

    /// <summary>
    /// Helper class for working with UI element context menu.
    /// </summary>
    public static class ContextMenuHelper
    {
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
        /// Copy <paramref name="text"/> to clipboard.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void CopyToClipboard(List<string> text) => CopyToClipboard(string.Join("\r", text));

        /// <summary>
        /// Opens the specified <paramref name="path"/> with Windows Explorer.
        /// </summary>
        /// <param name="path">Path to open in Windows Explorer.</param>
        public static void OpenInExplorer(string path) => Process.Start("explorer", path);

        /// <summary>
        /// Shows the flyout placed in relation to the specified element using the specified options.
        /// </summary>
        /// <param name="sender">UIElement is a base class for most of the Windows Runtime UI objects.</param>
        /// <param name="flyout">Flyout displayed on UI element.</param>
        /// <param name="args">Provides event data for the ContextRequested event.</param>
        public static void ShowContextMenu(UIElement sender, CommandBarFlyout flyout, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
        {
            args.Handled = true;
            Point point;
            _ = args.TryGetPosition(sender, out point);
            var options = new FlyoutShowOptions()
            {
                Position = point,
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = FlyoutShowMode.Standard,
            };
            flyout.ShowAt((FrameworkElement)sender, options);
        }
    }
}
