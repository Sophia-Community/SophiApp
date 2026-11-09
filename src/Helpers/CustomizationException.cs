// <copyright file="CustomizationException.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
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
    }
}
