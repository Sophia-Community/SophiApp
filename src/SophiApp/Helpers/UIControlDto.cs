// <copyright file="UIControlDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
#pragma warning disable CS8618 // A field that does not allow NULL values must contain a value other than NULL when exiting the constructor.
    /// <summary>
    /// Data transfer object for <see cref="UIControl"/>.
    /// </summary>
    public class UIControlDto
    {
        /// <summary>
        /// Gets or sets unique name.
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets unique tag.
        /// </summary>
        public UITag Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether support windows 10.
        /// </summary>
        public bool Windows10 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether support windows 11.
        /// </summary>
        public bool Windows11 { get; set; }

        /// <summary>
        /// Gets or sets localized headers.
        /// </summary>
        public Dictionary<string, string> Header { get; set; }

        /// <summary>
        /// Gets or sets localized descriptions.
        /// </summary>
        public Dictionary<string, string> Description { get; set; }

        /// <summary>
        /// Gets or sets header in the current or default localization.
        /// </summary>
        public string LocalizedHeader { get; set; }

        /// <summary>
        /// Gets or sets description in the current or default localization.
        /// </summary>
        public string LocalizedDecription { get; set; }

        /// <summary>
        /// Gets or sets child elements, if they exist.
        /// </summary>
        public List<UIControlChildDto>? Childs { get; set; }
    }
#pragma warning restore CS8618 // A field that does not allow NULL values must contain a value other than NULL when exiting the constructor.
}
