// <copyright file="SupportedOs.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Data transfer object for supported OS builds.
    /// </summary>
    public class SupportedOs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedOs"/> class.
        /// </summary>
        public SupportedOs()
        {
            MajorBuild = 26300;
            MinorBuild = 9974;
            LTSC_MajorBuild = 26100;
            LTSC_MinorBuild = 9974;
        }

        /// <summary>
        /// Gets or sets supported Microsoft Windows 11 major build.
        /// </summary>
        [JsonPropertyName("Windows_11_Major")]
        public int MajorBuild { get; set; }

        /// <summary>
        /// Gets or sets supported Microsoft Windows 11 minor build.
        /// </summary>
        [JsonPropertyName("Windows_11_Minor")]
        public int MinorBuild { get; set; }

        /// <summary>
        /// Gets or sets supported Microsoft Windows 11 LTSC 2024 major build.
        /// </summary>
        [JsonPropertyName("Windows_11_LTSC_Major")]
        public int LTSC_MajorBuild { get; set; }

        /// <summary>
        /// Gets or sets supported Microsoft Windows 11 LTSC 2024 minor build.
        /// </summary>
        [JsonPropertyName("Windows_11_LTSC_Minor")]
        public int LTSC_MinorBuild { get; set; }
    }
}
