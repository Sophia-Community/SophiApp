// <copyright file="OsProperties.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Management;
    using Microsoft.Win32;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Encapsulates OS properties.
    /// </summary>
    public record OsProperties(string Caption, int BuildNumber, int UpdateBuildRevision)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class.
        /// </summary>
        /// <param name="properties">A collection of WMI class properties.</param>
        public OsProperties(PropertyDataCollection properties)
            : this(
                  Caption: (string)properties[nameof(Caption)].Value,
                  BuildNumber: int.Parse((string)properties[nameof(BuildNumber)].Value),
                  UpdateBuildRevision: (int?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("UBR") ?? -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class with default values.
        /// </summary>
        public OsProperties()
            : this(
                  Caption: string.Empty,
                  BuildNumber: -1,
                  UpdateBuildRevision: -1)
        {
        }
    }

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

}
