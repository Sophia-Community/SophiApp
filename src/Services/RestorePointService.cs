// <copyright file="RestorePointService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Management;
    using System.ServiceProcess;
    using System.Text.RegularExpressions;
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public partial class RestorePointService : IRestorePointService
    {
        private const string Description = "SophiApp for Windows";
        private const string PolicyKey = "SOFTWARE\\Policies\\Microsoft\\Windows NT\\SystemRestore";
        private const string SettingsKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SystemRestore";
        private const string FrequencyValue = "SystemRestorePointCreationFrequency";

        // The volumes with system protection turned on
        private const string ProtectedVolumesKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SPP\\Clients";
        private const string ProtectedVolumesValue = "{09F7EDC5-294E-4180-AF6A-FB0E6A0E9513}";

        // MODIFY_SETTINGS and BEGIN_SYSTEM_CHANGE
        private const int RestorePointType = 12;
        private const int EventType = 100;
        private const double Gigabyte = 1024d * 1024 * 1024;

        private readonly IGroupPolicyService groupPolicyService;
        private readonly IOsService osService;
        private readonly IProcessService processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestorePointService"/> class.
        /// </summary>
        /// <param name="groupPolicyService">A service for working with group policy API.</param>
        /// <param name="osService">A service for working with Windows services API.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        public RestorePointService(IGroupPolicyService groupPolicyService, IOsService osService, IProcessService processService)
        {
            this.groupPolicyService = groupPolicyService;
            this.osService = osService;
            this.processService = processService;
        }

        private static string SystemDrive => Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))!;

        /// <inheritdoc/>
        public void Create()
        {
            // The policies that turn System Restore off are removed
            foreach (var name in new[] { "DisableSR", "DisableConfig" })
            {
                Registry.LocalMachine.DeleteSubKeyValue(PolicyKey, name);
                groupPolicyService.ClearPolicyCache(LGPOScope.Computer, PolicyKey, name);
            }

            // Restore points need both services
            foreach (var name in new[] { "VSS", "swprv" })
            {
                using var service = new System.ServiceProcess.ServiceController(name);

                if (service.StartType == ServiceStartMode.Disabled)
                {
                    osService.SetStartMode(service, ServiceStartMode.Manual);
                }
            }

            if (!SystemProtectionIsOn())
            {
                Invoke("Enable", ("Drive", SystemDrive));
            }

            // Windows skips a restore point if another one was created in the last 24 hours. This one is never skipped
            using (var settings = Registry.LocalMachine.CreateSubKey(SettingsKey))
            {
                settings.SetValue(FrequencyValue, 0, RegistryValueKind.DWord);
            }

            try
            {
                Invoke("CreateRestorePoint", ("Description", Description), ("RestorePointType", RestorePointType), ("EventType", EventType));
            }
            finally
            {
                Registry.LocalMachine.DeleteSubKeyValue(SettingsKey, FrequencyValue);
            }
        }

        /// <inheritdoc/>
        public double GetShadowStorageGigabytes()
        {
            try
            {
                // The volume Windows is started from
                if (GetVolumeId("BootVolume = TRUE") is not string volumeId)
                {
                    return 0;
                }

                // There is no shadow storage if system protection has never been turned on
                using var searcher = new ManagementObjectSearcher("root/CIMV2", "SELECT Volume, UsedSpace FROM Win32_ShadowStorage");

                foreach (var storage in searcher.Get().Cast<ManagementObject>())
                {
                    using (storage)
                    {
                        if (VolumeId().Match(storage["Volume"] as string ?? string.Empty).Value.Equals(volumeId, StringComparison.OrdinalIgnoreCase))
                        {
                            return Math.Round(Convert.ToUInt64(storage["UsedSpace"]) / Gigabyte, 2);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is ManagementException or System.Runtime.InteropServices.COMException or UnauthorizedAccessException)
            {
                App.Logger.LogSkippedException("Reading the shadow storage of the system drive", ex);
            }

            return 0;
        }

        /// <inheritdoc/>
        public void OpenSettings()
            => processService.StartProcessByName(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "SystemPropertiesProtection.exe"));

        // The part of a volume path that names the volume: Volume{GUID}
        [GeneratedRegex(@"Volume\{[0-9a-fA-F-]+\}")]
        private static partial Regex VolumeId();

        private static string? GetVolumeId(string condition)
        {
            using var searcher = new ManagementObjectSearcher("root/CIMV2", $"SELECT DeviceID FROM Win32_Volume WHERE {condition}");

            foreach (var volume in searcher.Get().Cast<ManagementObject>())
            {
                using (volume)
                {
                    var id = VolumeId().Match(volume["DeviceID"] as string ?? string.Empty);

                    if (id.Success)
                    {
                        return id.Value;
                    }
                }
            }

            return null;
        }

        private static bool SystemProtectionIsOn()
        {
            if (GetVolumeId($"DriveLetter = '{SystemDrive.TrimEnd('\\')}'") is not string volumeId)
            {
                return false;
            }

            var volumes = Registry.LocalMachine.GetSubKeyValue(ProtectedVolumesKey, ProtectedVolumesValue) as string[] ?? [];
            return Array.Exists(volumes, volume => volume.Contains(volumeId, StringComparison.OrdinalIgnoreCase));
        }

        // A method of the System Restore class of Windows. Anything but zero is an error
        private static void Invoke(string method, params (string Name, object Value)[] arguments)
        {
            using var restore = new ManagementClass("root/default:SystemRestore");
            using var parameters = restore.GetMethodParameters(method);

            foreach (var (name, value) in arguments)
            {
                parameters[name] = value;
            }

            ManagementBaseObject result;

            try
            {
                result = restore.InvokeMethod(method, parameters, null);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                // Such an exception has no text, only the code of the error
                throw new InvalidOperationException($"SystemRestore.{method} failed with the error 0x{ex.HResult:X8}", ex);
            }

            using var disposable = result;
            var code = Convert.ToUInt32(result["ReturnValue"]);

            if (code != 0)
            {
                throw new InvalidOperationException($"SystemRestore.{method} returned {code}");
            }
        }
    }
}
