// <copyright file="SettingLooks.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    /// <summary>
    /// How the card of a setting looks: the mark of a new setting, and the icon of a card that has no logo of its own.
    /// </summary>
    internal static class SettingLooks
    {
        // The settings added in the version that is being released: their cards have the mark "New".
        // The list is emptied when the next version gets its own new settings
        private static readonly HashSet<string> NewSettings =
        [
            nameof(Accessors.MouseAcceleration),
        ];

        // Glyphs of the font Segoe Fluent Icons
        private static readonly Dictionary<string, string> Glyphs = new()
        {
            [nameof(Accessors.ScanRegistryPolicies)] = "",
        };

        /// <summary>
        /// Gets a value indicating whether a setting is marked as new.
        /// </summary>
        /// <param name="setting">The name of the setting.</param>
        public static bool IsNew(string setting) => NewSettings.Contains(setting);

        /// <summary>
        /// Gets the glyph of the icon of a setting, or null if the setting has none.
        /// </summary>
        /// <param name="setting">The name of the setting.</param>
        public static string? GetGlyph(string setting) => Glyphs.GetValueOrDefault(setting);
    }
}
