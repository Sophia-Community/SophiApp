// <copyright file="AppLanguage.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Text.Json;
    using Microsoft.Windows.Globalization;
    using SophiApp.Extensions;

    /// <summary>
    /// A language of the app interface.
    /// </summary>
    public class AppLanguage
    {
        /// <summary>
        /// The name of the language in the settings file.
        /// </summary>
        public const string SettingsKey = "AppLanguage";

        /// <summary>
        /// The tag that stands for the language of Windows.
        /// </summary>
        public const string WindowsLanguageTag = "";

        private const string SettingsFile = "Settings.json";

        // The languages the app is translated into
        private static readonly string[] Tags = ["en-US", "ru-RU"];

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLanguage"/> class.
        /// </summary>
        /// <param name="tag">The language tag, such as "en-US".</param>
        /// <param name="name">The name of the language to show.</param>
        public AppLanguage(string tag, string name)
        {
            Tag = tag;
            Name = name;
        }

        /// <summary>
        /// Gets the tag of the language the app has used since it started.
        /// </summary>
        public static string Applied { get; private set; } = WindowsLanguageTag;

        /// <summary>
        /// Gets or sets the tag of the language chosen in the settings. It is used when the app starts next time.
        /// </summary>
        public static string Chosen { get; set; } = WindowsLanguageTag;

        /// <summary>
        /// Gets the language tag, such as "en-US".
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets the name of the language to show.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the app is translated into a language.
        /// </summary>
        /// <param name="tag">A language tag.</param>
        public static bool IsSupported(string? tag) => tag is not null && Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Makes the app use the language saved in the settings file. It has to be done before the first text is loaded,
        /// so the file is read here, not by the settings service. Without a saved language the app follows Windows.
        /// </summary>
        public static void ApplySaved()
        {
            try
            {
                var file = Path.Combine(EnvironmentHelper.AppFolder, SettingsFile);

                if (!File.Exists(file))
                {
                    return;
                }

                // Every setting is kept as a JSON text of its own
                var settings = Json.ToObject<Dictionary<string, string>>(File.ReadAllText(file));

                if (settings is not null && settings.TryGetValue(SettingsKey, out var saved) && Json.ToObject<string>(saved) is string tag && IsSupported(tag))
                {
                    ApplicationLanguages.PrimaryLanguageOverride = tag;
                    Applied = tag;
                    Chosen = tag;
                }
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                // A damaged settings file is not a reason to fail: the app follows the language of Windows
            }
        }
    }
}
