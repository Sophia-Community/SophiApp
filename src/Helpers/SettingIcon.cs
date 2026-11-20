// <copyright file="SettingIcon.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Creates the icon of a settings card by the icon index from the UI markup.
    /// </summary>
    internal static class SettingIcon
    {
        // The position in the list is the icon index
        private static readonly string[] Files = ["Windows11.svg", "Folder.svg", "OneDrive.svg", "DotNet.svg", "VisualC.svg", "WSL.svg"];

        /// <summary>
        /// Creates an icon. An unknown index gives the folder icon.
        /// </summary>
        /// <param name="iconIndex">The icon index from the UI markup.</param>
        public static ImageIcon Create(int iconIndex)
        {
            var file = iconIndex >= 0 && iconIndex < Files.Length ? Files[iconIndex] : "Folder.svg";
            return new ImageIcon { Source = new SvgImageSource(new Uri($"ms-appx:///Assets/{file}")) };
        }
    }
}
