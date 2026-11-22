// <copyright file="AppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Collections.Generic;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using Windows.ApplicationModel;
    using Windows.Management.Deployment;

    /// <inheritdoc/>
    public class AppxPackagesService : IAppxPackagesService
    {
        // These apps are hidden from the list of the apps to uninstall
        private static readonly HashSet<string> ExcludedPackages = new(StringComparer.OrdinalIgnoreCase)
        {
            // Dolby Access
            "DolbyLaboratories.DolbyAccess",
            "DolbyLaboratories.DolbyDigitalPlusDecoderOEM",
            // AMD Radeon Software
            "AdvancedMicroDevicesInc-2.AMDRadeonSoftware",
            // Intel Graphics Control Center
            "AppUp.IntelGraphicsControlPanel",
            "AppUp.IntelGraphicsExperience",
            // ELAN Touchpad
            "ELANMicroelectronicsCorpo.ELANTouchpadforThinkpad",
            "ELANMicroelectronicsCorpo.ELANTrackPointforThinkpa",
            // Microsoft Application Compatibility Enhancements
            "Microsoft.ApplicationCompatibilityEnhancements",
            // AVC Encoder Video Extension
            "Microsoft.AVCEncoderVideoExtension",
            // Microsoft Desktop App Installer
            "Microsoft.DesktopAppInstaller",
            // Store Experience Host
            "Microsoft.StorePurchaseApp",
            "MicrosoftWindows.Client.AIX",
            // Cross Device Experience Host
            "MicrosoftWindows.CrossDevice",
            // Notepad
            "Microsoft.WindowsNotepad",
            // Microsoft Store
            "Microsoft.WindowsStore",
            // Windows Terminal
            "Microsoft.WindowsTerminal",
            "Microsoft.WindowsTerminalPreview",
            // Web Media Extensions
            "Microsoft.WebMediaExtensions",
            // AV1 Video Extension
            "Microsoft.AV1VideoExtension",
            // Windows Subsystem for Linux
            "MicrosoftCorporationII.WindowsSubsystemForLinux",
            // HEVC Video Extensions from Device Manufacturer
            "Microsoft.HEVCVideoExtension",
            // Raw Image Extension
            "Microsoft.RawImageExtension",
            // HEIF Image Extensions
            "Microsoft.HEIFImageExtension",
            // MPEG-2 Video Extension
            "Microsoft.MPEG2VideoExtension",
            // VP9 Video Extensions
            "Microsoft.VP9VideoExtensions",
            // Webp Image Extensions
            "Microsoft.WebpImageExtension",
            // PowerShell
            "Microsoft.PowerShell",
            // NVIDIA Control Panel
            "NVIDIACorp.NVIDIAControlPanel",
            // Realtek Audio Console
            "RealtekSemiconductorCorp.RealtekAudioControl",
            // Synaptics
            "SynapticsIncorporated.SynapticsControlPanel",
            "SynapticsIncorporated.241916F58D6E7",
            "ELANMicroelectronicsCorpo.TrackPoint",
            // XBOX Game UI
            "Microsoft.XboxGameCallableUI",
        };

        // These apps have no bundle, so they are added to the list by name
        private static readonly string[] PackagesWithoutBundle =
        [
            // Outlook
            "Microsoft.OutlookForWindows",

            // Microsoft Teams
            "MSTeams",

            // XBOX Game Bar
            "Microsoft.XboxGamingOverlay",
        ];

        private readonly PackageManager packageManager = new();
        private readonly IPowerShellService powerShellService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppxPackagesService"/> class.
        /// </summary>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        public AppxPackagesService(IPowerShellService powerShellService) => this.powerShellService = powerShellService;

        /// <inheritdoc/>
        public bool PackageExist(string packageId, bool allUsers = false)
        {
            var packages = allUsers ? packageManager.FindPackages() : packageManager.FindPackagesForUser(string.Empty);
            return packages.Any(package => package.Id.Name.Equals(packageId, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public Package? GetPackageOrDefault(string packageId)
            => packageManager.FindPackagesForUser(string.Empty).FirstOrDefault(package => package.Id.Name.Equals(packageId));

        /// <inheritdoc/>
        public List<UwpPackage> GetRemovablePackages(bool allUsers)
        {
            // Names and logos are looked up among the packages of all users
            var allPackages = packageManager.FindPackages().ToLookup(package => package.Id.Name, StringComparer.OrdinalIgnoreCase);
            var userPackages = allUsers ? allPackages : packageManager.FindPackagesForUser(string.Empty).ToLookup(package => package.Id.Name, StringComparer.OrdinalIgnoreCase);

            // Only the properties that are needed are returned: whole package objects take much longer to transfer
            var properties = "Name, PackageFullName, NonRemovable, IsFramework, SignatureKind";
            var packages = powerShellService.Invoke(allUsers ? $"Get-AppxPackage -PackageTypeFilter Bundle -AllUsers | Select-Object -Property {properties}" : $"Get-AppxPackage -PackageTypeFilter Bundle | Select-Object -Property {properties}")
                .Where(bundle => !bundle.GetPropertyValue<bool>("NonRemovable") && !bundle.GetPropertyValue<bool>("IsFramework") && !IsSystemSignature(bundle.GetPropertyValue<string>("SignatureKind")))
                .Select(bundle => (Name: bundle.GetPropertyValue<string>("Name") ?? string.Empty, FullName: bundle.GetPropertyValue<string>("PackageFullName") ?? string.Empty))
                .Where(bundle => bundle.Name.Length > 0 && bundle.FullName.Length > 0)
                .ToList();

            // An app that is already in the list as a bundle is removed along with the bundle
            foreach (var name in PackagesWithoutBundle.Where(name => !packages.Exists(package => package.Name.Equals(name, StringComparison.OrdinalIgnoreCase))))
            {
                packages.AddRange(userPackages[name].Select(package => (package.Id.Name, package.Id.FullName)));
            }

            return packages
                .GroupBy(package => package.Name, StringComparer.OrdinalIgnoreCase)
                .Where(group => !ExcludedPackages.Contains(group.Key))
                .Select(group => new UwpPackage(
                    name: group.Key,
                    displayName: allPackages[group.Key].Select(GetDisplayName).FirstOrDefault(name => name.Length > 0) ?? group.Key,
                    logo: allPackages[group.Key].Select(GetLogo).FirstOrDefault(logo => logo is not null),
                    fullNames: group.Select(package => package.FullName).Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList()))
                .OrderBy(package => package.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        /// <inheritdoc/>
        public void RemovePackage(string packageId, bool allUsers = false)
        {
            var bundles = allUsers
                ? packageManager.FindPackagesWithPackageTypes(PackageTypes.Bundle)
                : packageManager.FindPackagesForUserWithPackageTypes(string.Empty, PackageTypes.Bundle);

            RemovePackages(bundles.Where(bundle => bundle.Id.Name.Contains(packageId, StringComparison.OrdinalIgnoreCase)).Select(bundle => bundle.Id.FullName).ToList(), allUsers);
        }

        /// <inheritdoc/>
        public void RemovePackages(IEnumerable<string> packageFullNames, bool allUsers)
        {
            foreach (var packageFullName in packageFullNames)
            {
                try
                {
                    // What the cmdlet Remove-AppxPackage calls
                    var removal = packageManager.RemovePackageAsync(packageFullName, allUsers ? RemovalOptions.RemoveForAllUsers : RemovalOptions.None).AsTask().GetAwaiter().GetResult();

                    if (removal.ExtendedErrorCode is { } error)
                    {
                        App.Logger.LogSkippedException($"Removing the UWP package {packageFullName}", error);
                    }
                }
                catch (Exception ex)
                {
                    // A package that cannot be removed does not stop the others
                    App.Logger.LogSkippedException($"Removing the UWP package {packageFullName}", ex);
                }
            }
        }

        // The signature kind arrives either as its name or as its number
        private static bool IsSystemSignature(string? signatureKind) => signatureKind is "System" or "4";

        private static string GetDisplayName(Package package)
        {
            try
            {
                return package.DisplayName ?? string.Empty;
            }
            catch (Exception ex)
            {
                // The name of a damaged package cannot be read
                App.Logger.LogSkippedException("Reading the name of a UWP package", ex);
                return string.Empty;
            }
        }

        private static Uri? GetLogo(Package package)
        {
            try
            {
                return File.Exists(package.Logo.LocalPath) ? package.Logo : null;
            }
            catch (Exception ex)
            {
                // The logo of a damaged package cannot be read
                App.Logger.LogSkippedException("Reading the logo of a UWP package", ex);
                return null;
            }
        }
    }
}
