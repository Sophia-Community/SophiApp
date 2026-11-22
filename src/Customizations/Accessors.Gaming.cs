// <copyright file="Accessors.Gaming.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.ServiceProcess;
    using System.Text;
    using Windows.ApplicationModel;

    /// <content>
    /// The settings of the "Gaming" page.
    /// </content>
    public static partial class Accessors
    {
        /// <summary>
        /// Gets XBOX game bar state.
        /// </summary>
        public static bool XBOXGameBar()
        {
            var appCaptureIsEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", "AppCaptureEnabled") as int? ?? -1;
            var dvrIsEnabled = Registry.CurrentUser.GetSubKeyValue("System\\GameConfigStore", "GameDVR_Enabled") as int? ?? -1;

            if (appCaptureIsEnabled == 0 && dvrIsEnabled == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets XBOX game tips state.
        /// </summary>
        public static bool XBOXGameTips()
        {
            if (AppxPackagesService.PackageExist("Microsoft.XboxGamingOverlay"))
            {
                var startupPanelIsEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\GameBar", "ShowStartupPanel") as int? ?? -1;
                return startupPanelIsEnabled != 0;
            }

            throw new CustomizationException(exceptionMessage: "XBOX Game Bar package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled") { Arguments = ["XBOX Game Bar"] };
        }

        /// <summary>
        /// Get GPU scheduling state.
        /// </summary>
        public static bool GPUScheduling()
        {
            const int WDDMMinimalVersion = 2700;
            // Determining whether PC has an external graphics card
            var isExternalDAC = InstrumentationService.DetectDACType();
            var isVM = InstrumentationService.DetectVM();
            var wddmVersion = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\\FeatureSetUsage", "WddmVersion_Min") as int? ?? -1;

            var hwSchMode = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode") as int? ?? -1;

            // The checks below are required to enable the feature only, so it can always be disabled
            if (hwSchMode == 2)
            {
                return true;
            }

            // Checking whether a WDDM version is 2.7 or higher
            if (isExternalDAC && !isVM && wddmVersion >= WDDMMinimalVersion)
            {
                return false;
            }

            if (isVM)
            {
                throw new CustomizationException(exceptionMessage: "Windows is running on a virtual machine", resourceKey: "UIModel_Exception_GPUScheduling_VirtualMachine");
            }

            if (!isExternalDAC)
            {
                throw new CustomizationException(exceptionMessage: "No dedicated graphics card found", resourceKey: "UIModel_Exception_GPUScheduling_DedicatedGPUNotFound");
            }

            // The version is kept in thousandths: 2700 is 2.7. The dot stays a dot in every locale
            var currentVersion = (wddmVersion / 1000d).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
            throw new CustomizationException(exceptionMessage: $"Supported WDDM version is {WDDMMinimalVersion} and higher. Current WDDM version is {wddmVersion}", resourceKey: "UIModel_Exception_GPUScheduling_WDDMVersion") { Arguments = [currentVersion] };
        }
    }
}
