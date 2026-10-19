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
            Build = -1;
            DisplayVersion = "N/A";
            UBR = -1;
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
            Build = int.Parse((string?)properties["BuildNumber"]?.Value ?? "-1");
            DisplayVersion = (string?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("DisplayVersion") ?? "N/A";
            UBR = (int?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("UBR") ?? -1;
            Edition = (string?)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("EditionID") ?? "N/A";
            ComputerName = (string?)properties["CSName"]?.Value ?? "N/A";
        }

        /// <summary>
        /// Gets OS caption version.
        /// </summary>
        public string Caption { get; init; }

        /// <summary>
        /// Gets OS build version.
        /// </summary>
        public int Build { get; init; }

        /// <summary>
        /// Gets OS display version e.g. 25H2.
        /// </summary>
        public string DisplayVersion { get; init; }

        /// <summary>
        /// Gets OS update build revision version.
        /// </summary>
        public int UBR { get; init; }

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
