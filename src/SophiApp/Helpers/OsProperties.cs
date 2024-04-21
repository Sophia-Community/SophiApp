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
    public record OsProperties(
            string Caption,
            int BuildNumber,
            int UpdateBuildRevision,
            string Edition,
            string CSName)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class.
        /// </summary>
        /// <param name="properties">A collection of WMI class properties.</param>
        public OsProperties(PropertyDataCollection properties)
            : this(
                  Caption: (string?)properties[nameof(Caption)]?.Value ?? "n/a",
                  BuildNumber: int.Parse((string?)properties[nameof(BuildNumber)]?.Value ?? "-1"),
                  UpdateBuildRevision: (int?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("UBR") ?? -1,
                  Edition: (string?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("EditionID") ?? "n/a",
                  CSName: (string?)properties[nameof(CSName)]?.Value ?? "n/a")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class.
        /// </summary>
        public OsProperties()
            : this(
                  Caption: "n/a",
                  BuildNumber: -1,
                  UpdateBuildRevision: -1,
                  Edition: "n/a",
                  CSName: "n/a")
        {
        }
    }

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

}
