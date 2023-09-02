// <copyright file="UIControl.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// Implements the basic logic of the user interface control.
    /// </summary>
    public class UIControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIControl"/> class.
        /// </summary>
        /// <param name="dto">Dto for initialization.</param>
        public UIControl(UIControlDto dto)
        {
            Name = dto.Function;
            Tag = dto.Tag;
            Win10Supported = dto.Windows10;
            Win11Supported = dto.Windows11;
            Header = dto.LocalizedHeader;
            Description = dto.LocalizedDecription;
        }

        /// <summary>
        /// Gets <see cref="UIControl"/> unique name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets <see cref="UIControl"/> unique tag.
        /// </summary>
        public UITag Tag { get; init; }

        /// <summary>
        /// Gets a value indicating whether <see cref="UIControl"/> supported windows 10.
        /// </summary>
        public bool Win10Supported { get; init; }

        /// <summary>
        /// Gets a value indicating whether <see cref="UIControl"/> supported windows 11.
        /// </summary>
        public bool Win11Supported { get; init; }

        /// <summary>
        /// Gets <see cref="UIControl"/> localized header.
        /// </summary>
        public string Header { get; init; }

        /// <summary>
        /// Gets <see cref="UIControl"/> localized description.
        /// </summary>
        public string Description { get; init; }
    }
}
