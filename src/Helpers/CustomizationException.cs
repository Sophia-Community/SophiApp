// <copyright file="CustomizationException.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Globalization;
    using SophiApp.Extensions;

    /// <summary>
    /// Represents errors that occur during <see cref="Customizations"/> execution.
    /// </summary>
    public class CustomizationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomizationException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Message to write to the log.</param>
        /// <param name="resourceKey">Localized string resource key.</param>
        public CustomizationException(string exceptionMessage, string resourceKey)
            : base(exceptionMessage)
        {
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// Gets localized string resource key.
        /// </summary>
        public string ResourceKey { get; }

        /// <summary>
        /// Gets the state of a package that cannot be installed or uninstalled right now. Null if the state is unknown.
        /// </summary>
        public InstallState? InstallState { get; init; }

        /// <summary>
        /// Gets the values for the placeholders of the localized text: an app name, a version, an address.
        /// </summary>
        public object[] Arguments { get; init; } = [];

        /// <summary>
        /// Gets the localized text to show to the user.
        /// </summary>
        public string GetText()
        {
            var text = ResourceKey.GetLocalized();
            return Arguments.Length == 0 ? text : string.Format(CultureInfo.CurrentCulture, text, Arguments);
        }
    }
}
