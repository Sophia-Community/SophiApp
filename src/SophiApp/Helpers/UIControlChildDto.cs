// <copyright file="UIControlChildDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
#pragma warning disable CS8618 // A field that does not allow NULL values must contain a value other than NULL when exiting the constructor.
    /// <summary>
    /// Data transfer object for <see cref="UIControl"/> child.
    /// </summary>
    public record UIControlChildDto
    {
        /// <summary>
        /// Gets or sets localized headers.
        /// </summary>
        public List<KeyValuePair<string, string>> Header { get; set; }

        /// <summary>
        /// Gets or sets localized descriptions.
        /// </summary>
        public List<KeyValuePair<string, string>> Description { get; set; }

        /// <summary>
        /// Gets or sets header in the current or default localization.
        /// </summary>
        public string LocalizedHeader { get; set; }

        /// <summary>
        /// Gets or sets description in the current or default localization.
        /// </summary>
        public string LocalizedDecription { get; set; }
    }
#pragma warning restore CS8618 // A field that does not allow NULL values must contain a value other than NULL when exiting the constructor.
}
