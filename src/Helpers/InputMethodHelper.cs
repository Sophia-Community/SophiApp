// <copyright file="InputMethodHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Microsoft.Win32;

    /// <summary>
    /// The input methods of the language list of the user: keyboard layouts and input method editors.
    /// </summary>
    internal static partial class InputMethodHelper
    {
        /// <summary>
        /// The id of the choice "start with the first input method of the language list".
        /// </summary>
        public const string LanguageListId = "LanguageList";

        private const string ProfileKey = "Control Panel\\International\\User Profile";

        /// <summary>
        /// Gets the input methods in the order of the language list: the id such as "0409:00000409", the name of the language and the name of the keyboard layout.
        /// </summary>
        public static IReadOnlyList<(string Tip, string Language, string Layout)> GetInstalled()
        {
            var methods = new List<(string Tip, string Language, string Layout)>();
            using var profile = Registry.CurrentUser.OpenSubKey(ProfileKey);

            if (profile?.GetValue("Languages") is not string[] languages)
            {
                return methods;
            }

            foreach (var tag in languages)
            {
                using var language = profile.OpenSubKey(tag);

                if (language is null)
                {
                    continue;
                }

                // The input methods of a language are its values named by their ids; the data of a value is its place in the list
                var tips = language.GetValueNames()
                    .Where(IsTip)
                    .OrderBy(tip => language.GetValue(tip) is int order ? order : int.MaxValue);

                methods.AddRange(tips.Select(tip => (tip, GetLanguageName(tag), GetLayoutName(tip))));
            }

            return methods;
        }

        /// <summary>
        /// Gets a value indicating whether a text is the id of an input method: "0409:00000409" for a keyboard layout,
        /// or the language with two GUIDs for an input method editor.
        /// </summary>
        /// <param name="text">A text.</param>
        public static bool IsTip(string text) => TipPattern().IsMatch(text);

        [GeneratedRegex(@"^[0-9A-Fa-f]{4}:([0-9A-Fa-f]{8}|\{[0-9A-Fa-f-]{36}\}\{[0-9A-Fa-f-]{36}\})$")]
        private static partial Regex TipPattern();

        private static string GetLanguageName(string tag)
        {
            try
            {
                return CultureInfo.GetCultureInfo(tag).DisplayName;
            }
            catch (CultureNotFoundException)
            {
                return tag;
            }
        }

        // The name of a keyboard layout, such as "US". An input method editor has no layout
        private static string GetLayoutName(string tip)
        {
            var id = tip[(tip.IndexOf(':') + 1)..];

            if (id.Length != 8)
            {
                return string.Empty;
            }

            var key = $"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts\\{id}";
            var localizedName = IndirectString.Resolve(Registry.GetValue(key, "Layout Display Name", null) as string);
            return localizedName.Length > 0 ? localizedName : Registry.GetValue(key, "Layout Text", null) as string ?? string.Empty;
        }
    }
}
