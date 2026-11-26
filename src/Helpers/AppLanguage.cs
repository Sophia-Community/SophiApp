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
        /// Gets the tag of the language the app uses: the one it started with, or the one chosen while it runs.
        /// </summary>
        public static string Applied { get; private set; } = WindowsLanguageTag;

        /// <summary>
        /// Gets the tag of the language the texts are shown in. When the app follows Windows, it is the language Windows gave it.
        /// </summary>
        public static string Current
        {
            get
            {
                var language = AppResources.Instance.Language;

                return Array.Find(Tags, tag => tag.Equals(language, StringComparison.OrdinalIgnoreCase))
                    ?? Array.Find(Tags, tag => language.Length >= 2 && tag.StartsWith(language[..2], StringComparison.OrdinalIgnoreCase))
                    ?? Tags[0];
            }
        }

        /// <summary>
        /// Gets or sets the tag of the language chosen in the settings.
        /// </summary>
        public static string Chosen { get; set; } = WindowsLanguageTag;

        /// <summary>
        /// Gets the languages the app is translated into. Their names are not translated: everyone finds their own language.
        /// </summary>
        public static IReadOnlyList<AppLanguage> Translations { get; } = new AppLanguage[] { new("en-US", "English"), new("ru-RU", "Русский") };

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
        /// Makes the app use a language from now on. The texts that are already shown stay as they are until their page is created anew.
        /// </summary>
        /// <param name="tag">The tag of a language the app is translated into. An empty tag stands for the language of Windows.</param>
        public static void Apply(string tag)
        {
            // The texts remember the language of Windows before another one takes its place
            var resources = AppResources.Instance;

            // An empty value is not accepted, so the language of Windows is set by its tag
            var language = tag.Length > 0 ? tag : resources.WindowsLanguage;

            if (language.Length > 0)
            {
                ApplicationLanguages.PrimaryLanguageOverride = language;
            }

            resources.SetLanguage(tag);
            Applied = tag;
            Chosen = tag;
        }

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

                // The tag is kept as it is. The files of the earlier versions have it in quotes
                if (settings is not null
                    && settings.TryGetValue(SettingsKey, out var saved)
                    && (saved.StartsWith('"') ? Json.ToObject<string>(saved) : saved) is string tag
                    && IsSupported(tag))
                {
                    Apply(tag);
                }
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                // A damaged settings file is not a reason to fail: the app follows the language of Windows
            }
        }
    }
}
