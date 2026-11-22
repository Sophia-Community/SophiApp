// <copyright file="SettingLinks.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    /// <summary>
    /// The settings that write the same place in Windows. Chosen together with different values, the one applied later would win silently,
    /// so the choice in one of them sets and locks the other.
    /// </summary>
    internal static class SettingLinks
    {
        private static readonly SettingLink[] Links =
        [
            // Recommended troubleshooting needs optional diagnostic data and error reporting, and turns both on
            new(nameof(Mutators.RecommendedTroubleshooting), null, nameof(Mutators.DiagnosticDataLevel), 1),
            new(nameof(Mutators.RecommendedTroubleshooting), null, nameof(Mutators.ErrorReporting), true),

            // The Recommended section of Start is made of these lists
            new(nameof(Mutators.StartRecommendedSection), true, nameof(Mutators.RecentlyAddedStartApps), true),
            new(nameof(Mutators.StartRecommendedSection), true, nameof(Mutators.MostUsedStartApps), true),
            new(nameof(Mutators.StartRecommendedSection), true, nameof(Mutators.StartRecommendationsTips), true),
            new(nameof(Mutators.StartRecommendedSection), false, nameof(Mutators.RecentlyAddedStartApps), false),
            new(nameof(Mutators.StartRecommendedSection), false, nameof(Mutators.MostUsedStartApps), false),
            new(nameof(Mutators.StartRecommendedSection), false, nameof(Mutators.StartRecommendationsTips), false),

            // The tips are a part of XBOX Game Bar
            new(nameof(Mutators.XBOXGameTips), true, nameof(Mutators.XBOXGameBar), true),

            // Search highlights come from the web search
            new(nameof(Mutators.SearchHighlights), true, nameof(Mutators.BingSearch), true),
            new(nameof(Mutators.BingSearch), false, nameof(Mutators.SearchHighlights), false),
        ];

        /// <summary>
        /// Gets all links.
        /// </summary>
        public static IReadOnlyList<SettingLink> All => Links;
    }
}
