// <copyright file="MARGINS.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;
    #pragma warning disable S101 // Types should be named in PascalCase

    /// <summary>
    /// Represents app window margins.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        #pragma warning disable SA1300 // Element should begin with upper-case letter

        /// <summary>
        /// Gets or sets window left margin.
        /// </summary>
        public int cxLeftWidth { get; set; }

        /// <summary>
        /// Gets or sets window right margin.
        /// </summary>
        public int cxRightWidth { get; set; }

        /// <summary>
        /// Gets or sets window top margin.
        /// </summary>
        public int cyTopHeight { get; set; }

        /// <summary>
        /// Gets or sets window bottom margin.
        /// </summary>
        public int cyBottomHeight { get; set; }

        #pragma warning restore SA1300 // Element should begin with upper-case letter
    }

    #pragma warning restore S101 // Types should be named in PascalCase
}
