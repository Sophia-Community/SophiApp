// <copyright file="OsProperties.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Management;
    using Microsoft.Win32;

    /// <summary>
    /// Data transfer object for OS properties.
    /// </summary>
    public class OsProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class.
        /// </summary>
        public OsProperties()
        {
            Caption = "N/A";
            MajorBuild = -1;
            DisplayVersion = "N/A";
            MinorBuild = -1;
            Edition = "N/A";
            ComputerName = "N/A";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsProperties"/> class.
        /// </summary>
        /// <param name="properties">A collection of OS properties.</param>
        /// <param name="caption">OS caption version.</param>
        public OsProperties(PropertyDataCollection properties, string caption)
        {
            Caption = caption ?? "N/A";
            MajorBuild = int.Parse((string?)properties["BuildNumber"]?.Value ?? "-1");
            DisplayVersion = (string?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("DisplayVersion") ?? "N/A";
            MinorBuild = (int?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("UBR") ?? -1;
            Edition = (string?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("EditionID") ?? "N/A";
            ComputerName = (string?)properties["CSName"]?.Value ?? "N/A";
        }

        /// <summary>
        /// Gets OS caption version.
        /// </summary>
        public string Caption { get; init; }

        /// <summary>
        /// Gets OS major build version.
        /// </summary>
        public int MajorBuild { get; init; }

        /// <summary>
        /// Gets OS display version e.g. 26H2.
        /// </summary>
        public string DisplayVersion { get; init; }

        /// <summary>
        /// Gets OS minor build (Update Build Revision) version.
        /// </summary>
        public int MinorBuild { get; init; }

        /// <summary>
        /// Gets OS edition.
        /// </summary>
        public string Edition { get; init; }

        /// <summary>
        /// Gets PC name.
        /// </summary>
        public string ComputerName { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the OS is LTSC version.
        /// </summary>
        public bool IsLTSC { get; set; } = false;
    }
}
