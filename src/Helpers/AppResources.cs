// <copyright file="AppResources.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.Windows.ApplicationModel.Resources;
    using Windows.Foundation;

    /// <summary>
    /// The texts of the app. The pages and the code take them from here, so the language can be changed while the app runs.
    /// </summary>
    public sealed partial class AppResources : IResourceManager
    {
        private const string LanguageQualifier = "Language";

        // The pages may keep the contexts they were given, so a new language is set in each of them
        private const int KeptContexts = 16;

        private readonly ResourceManager manager = new();
        private readonly List<ResourceContext> contexts = [];
        private readonly Lock contextsLock = new();
        private readonly string windowsLanguages;
        private string chosenTag = string.Empty;

        private AppResources()
        {
            Strings = manager.MainResourceMap.GetSubtree("Resources");
            Context = manager.CreateResourceContext();

            // What Windows gives the app before any language is chosen in it
            windowsLanguages = Context.QualifierValues.TryGetValue(LanguageQualifier, out var languages) ? languages : string.Empty;
        }

        /// <inheritdoc/>
        public event TypedEventHandler<ResourceManager, ResourceNotFoundEventArgs> ResourceNotFound
        {
            add => manager.ResourceNotFound += value;
            remove => manager.ResourceNotFound -= value;
        }

        /// <summary>
        /// Gets the texts of the app.
        /// </summary>
        public static AppResources Instance { get; } = new();

        /// <summary>
        /// Gets the texts of the strings file.
        /// </summary>
        public ResourceMap Strings { get; }

        /// <summary>
        /// Gets the context the code takes the texts with.
        /// </summary>
        public ResourceContext Context { get; }

        /// <inheritdoc/>
        public ResourceMap MainResourceMap => manager.MainResourceMap;

        /// <summary>
        /// Gets the tag of the language the texts come in, such as "en-US".
        /// </summary>
        public string Language => Context.QualifierValues.TryGetValue(LanguageQualifier, out var languages)
            ? languages.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? string.Empty
            : string.Empty;

        /// <summary>
        /// Gets the tag of the language Windows gives the app when no language is chosen in it. Empty if Windows did not tell.
        /// </summary>
        public string WindowsLanguage => windowsLanguages.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? string.Empty;

        /// <inheritdoc/>
        public ResourceContext CreateResourceContext()
        {
            var context = manager.CreateResourceContext();

            lock (contextsLock)
            {
                if (chosenTag.Length > 0)
                {
                    context.QualifierValues[LanguageQualifier] = chosenTag;
                }

                contexts.Add(context);

                if (contexts.Count > KeptContexts)
                {
                    contexts.RemoveAt(0);
                }
            }

            return context;
        }

        /// <summary>
        /// Makes the texts come in a language from now on.
        /// </summary>
        /// <param name="tag">The language tag, such as "en-US". An empty tag stands for the language of Windows.</param>
        public void SetLanguage(string tag)
        {
            var languages = tag.Length > 0 ? tag : windowsLanguages;

            if (languages.Length == 0)
            {
                return;
            }

            lock (contextsLock)
            {
                chosenTag = languages;
                Context.QualifierValues[LanguageQualifier] = languages;

                foreach (var context in contexts)
                {
                    context.QualifierValues[LanguageQualifier] = languages;
                }
            }
        }
    }
}
