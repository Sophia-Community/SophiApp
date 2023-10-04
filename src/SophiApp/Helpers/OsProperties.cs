// <copyright file="OsProperties.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Management;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Encapsulates OS properties.
    /// </summary>
    public record OsProperties(string Caption)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class.
        /// </summary>
        /// <param name="properties">A collection of WMI class properties.</param>
        public OsProperties(PropertyDataCollection properties)
            : this((string)properties[nameof(Caption)].Value)
        {
        }
    }

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

}
