// <copyright file="EdgeShortcutPolicy.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// The policy that forbids Microsoft Edge to create a desktop shortcut when it is updated. Every channel of Edge has a value of its own.
    /// </summary>
    public static class EdgeShortcutPolicy
    {
        /// <summary>
        /// The registry key of the Microsoft Edge update policies.
        /// </summary>
        public const string Key = "SOFTWARE\\Policies\\Microsoft\\EdgeUpdate";

        /// <summary>
        /// The name of the stable channel as Windows shows it in the list of installed programs.
        /// </summary>
        public const string StableProgram = "Microsoft Edge";

        /// <summary>
        /// Gets the channels: the name of the program and the name of its policy value. The stable channel is the first one.
        /// </summary>
        public static IReadOnlyList<(string Program, string ValueName)> Channels { get; } = new (string Program, string ValueName)[]
        {
            (StableProgram, "CreateDesktopShortcut{56EB18F8-B008-4CBD-B6D2-8C97FE7E9062}"),
            ("Microsoft Edge Beta", "CreateDesktopShortcut{2CD8A007-E189-409D-A2C8-9AF4EF3C72AA}"),
            ("Microsoft Edge Dev", "CreateDesktopShortcut{0D50BFEC-CD6A-4F9A-964C-C7416E3ACB10}"),
            ("Microsoft Edge Canary", "CreateDesktopShortcut{65C35B14-6C1D-4122-AC46-7148CC9D6497}"),
        };

        /// <summary>
        /// Gets a value indicating whether the policy template of Microsoft Edge updates is installed. It is not a default one,
        /// and without it the Local Group Policy Editor has nowhere to show the policy.
        /// </summary>
        public static bool TemplateExists => File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "PolicyDefinitions\\msedgeupdate.admx"));
    }
}
