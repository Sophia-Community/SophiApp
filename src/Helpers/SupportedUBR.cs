// <copyright file="SupportedUBR.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    #pragma warning disable S101 // Types should be named in PascalCase
    using Newtonsoft.Json;

    /// <summary>
    /// Data transfer object for supported OS UBR.
    /// </summary>
    public class SupportedUBR
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedUBR"/> class.
        /// </summary>
        public SupportedUBR()
        {
            Win11 = -1;
            Win11LTSC = -1;
        }

        /// <summary>
        /// Gets or sets Microsoft Windows 11 supported update build revision.
        /// </summary>
        [JsonProperty("Windows_11")]
        public int Win11 { get; set; }

        /// <summary>
        /// Gets or sets Microsoft Windows 11 LTSC 2024 supported update build revision.
        /// </summary>
        [JsonProperty("Windows_11_LTSC_2024")]
        public int Win11LTSC { get; set; }
    }
    #pragma warning restore S101 // Types should be named in PascalCase
}
