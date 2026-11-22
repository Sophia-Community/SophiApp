// <copyright file="Mutators.Gaming.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using System.Text.Json.Nodes;
    using TaskScheduler = Microsoft.Win32.TaskScheduler;

    /// <content>
    /// The settings of the "Gaming" page.
    /// </content>
    public static partial class Mutators
    {
        /// <summary>
        /// Set XBOX game bar state.
        /// </summary>
        /// <param name="enable">XBOX game bar state.</param>
        public static void XBOXGameBar(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", "AppCaptureEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
            Registry.CurrentUser.SetValueIfKeyExists("System\\GameConfigStore", "GameDVR_Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set XBOX game tips state.
        /// </summary>
        /// <param name="enable">XBOX game tips state.</param>
        public static void XBOXGameTips(bool enable)
        {
            if (enable)
            {
                // We need to turn XBOX Game Bar on first
                Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", "AppCaptureEnabled", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.SetValueIfKeyExists("System\\GameConfigStore", "GameDVR_Enabled", 1, RegistryValueKind.DWord);
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\GameBar", "ShowStartupPanel", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set GPU scheduling state.
        /// </summary>
        /// <param name="enable">GPU scheduling state.</param>
        public static void GPUScheduling(bool enable)
        {
            Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode", enable ? 2 : 1, RegistryValueKind.DWord);
        }
    }
}
