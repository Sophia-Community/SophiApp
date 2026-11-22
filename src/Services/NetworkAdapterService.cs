// <copyright file="NetworkAdapterService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Management.Infrastructure;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;
    using System.Diagnostics;
    using System.Globalization;
    using System.Management;
    using System.Text;
    using System.Xml.Linq;

    /// <inheritdoc/>
    public class NetworkAdapterService : INetworkAdapterService
    {
        // The WMI classes of the network cmdlets (Get-NetAdapter, Get-NetAdapterPowerManagement, Get-NetConnectionProfile) are asked directly:
        // the cmdlets only wrap them, and their modules take seconds to load
        private const string CimNamespace = "root/StandardCimv2";

        // InterfaceOperationalStatus of MSFT_NetAdapter: the adapter status is "Up"
        private const uint OperationalStatusUp = 1;

        // NdisPhysicalMedium of MSFT_NetAdapter: Native 802.11
        // https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh968170(v=vs.85)
        private const uint WiFiPhysicalMedium = 9;

        // IPv4Connectivity and IPv6Connectivity of MSFT_NetConnectionProfile: Internet
        private const uint InternetConnectivity = 4;

        // AllowComputerToTurnOffDevice of MSFT_NetAdapterPowerManagementSettingData
        private const uint TurnOffDeviceUnsupported = 0;
        private const uint TurnOffDeviceDisabled = 1;
        private const uint TurnOffDeviceEnabled = 2;

        /// <inheritdoc/>
        public IReadOnlyList<(string Name, string Title, string Description, bool IsEnabled, bool IsConnected)> GetTurnOffDevice()
        {
            try
            {
                // Only the adapters that have AllowComputerToTurnOffDevice property to manage. The connected ones go first
                var physicalAdapters = GetPhysicalAdapters();

                return GetPowerManagement(physicalAdapters)
                    .Select(settings => (Settings: settings, Adapter: physicalAdapters.First(adapter => adapter.Name == settings.Name)))

                    // One device can be several adapters in Windows, as a Wi-Fi card with two links. The setting belongs to the device
                    .GroupBy(item => item.Adapter.DeviceId, StringComparer.OrdinalIgnoreCase)
                    .Select(device => device.OrderByDescending(item => item.Adapter.IsUp).ThenBy(item => item.Adapter.Name, StringComparer.CurrentCultureIgnoreCase).ToList())
                    .OrderByDescending(device => device[0].Adapter.IsUp)
                    .Select(device => (
                        device[0].Settings.Name,
                        string.Join(", ", device.Select(item => item.Settings.Name)),
                        device[0].Adapter.Description,
                        device[0].Settings.State == TurnOffDeviceEnabled,
                        device[0].Adapter.IsUp))
                    .ToList();
            }
            catch (Exception ex) when (IsWmiError(ex))
            {
                // It is the same for the user as no adapter to manage
                App.Logger.LogSkippedException("Reading the power management of the network adapters from WMI", ex);
                return [];
            }
        }

        /// <inheritdoc/>
        public void SetTurnOffDevice(IReadOnlyDictionary<string, bool> adapters)
        {
            var physicalAdapters = GetPhysicalAdapters();
            var managedNames = GetPowerManagement(physicalAdapters).Select(settings => settings.Name).ToList();

            // A choice is made for a device, and every adapter of that device gets it
            var states = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (var adapter in physicalAdapters.Where(adapter => managedNames.Contains(adapter.Name)))
            {
                var chosen = physicalAdapters.Find(other => other.DeviceId.Equals(adapter.DeviceId, StringComparison.OrdinalIgnoreCase) && adapters.ContainsKey(other.Name));

                if (chosen.Name is not null)
                {
                    states[adapter.Name] = adapters[chosen.Name];
                }
            }

            var adapterNames = states.Keys.ToList();

            // Check whether adapters have AllowComputerToTurnOffDevice property to manage
            if (adapterNames.Count == 0)
            {
                return;
            }

            string? ssid = null;
            string? profileName = null;

            // Only the connected adapters lose a connection when they restart, so only they are waited for
            var connectedAdapters = physicalAdapters.Where(adapter => adapter.IsUp && adapterNames.Contains(adapter.Name)).Select(adapter => adapter.Name).ToList();

            // Connected Wi-Fi adapters whose power management will be changed
            var wiFiAdapters = physicalAdapters.Where(adapter => adapter.IsWiFi && adapter.IsUp && adapterNames.Contains(adapter.Name)).Select(adapter => adapter.Name).ToList();

            // Check whether PC is currently connected to a Wi-Fi network
            if (wiFiAdapters.Count > 0)
            {
                // Get currently connected Wi-Fi network SSID
                ssid = GetConnectedSsid();

                if (ssid is null)
                {
                    // Network shell commands need location permission to access WLAN information
                    ExplorerHelper.Open("ms-settings:privacy-location");
                    throw new CustomizationException(exceptionMessage: "Location services are turned off, so the connected Wi-Fi network cannot be determined", resourceKey: "UIModel_Exception_LocationServicesDisabled");
                }

                // Get the WLAN profile name by SSID, since they may differ. If the profile was not found, SSID is used
                profileName = GetWlanProfileName(ssid) ?? ssid;
            }

            // Check whether PC has Internet access before adapters restart
            // If not (no Internet or NCSI active probing is disabled), we will wait only until adapters are up
            var hadInternet = HasInternetAccess();

            try
            {
                foreach (var adapterName in adapterNames)
                {
                    SetPowerManagement(adapterName, states[adapterName]);
                }
            }
            catch (CimException ex)
            {
                // The setting stays available: the user can try again
                throw new CustomizationException(exceptionMessage: $"The power management of a network adapter was not changed: {ex.Message}", resourceKey: "UIModel_Exception_ApplyFailed");
            }

            if (connectedAdapters.Count > 0)
            {
                WaitForRestart(connectedAdapters);
                WaitForConnection(connectedAdapters, wiFiAdapters, hadInternet, ssid, profileName);
            }
        }

        private static bool IsWmiError(Exception ex) => ex is ManagementException || ex is System.Runtime.InteropServices.COMException || ex is UnauthorizedAccessException;

        // A name inside the quotes of a WMI query
        private static string ToQueryText(string name) => name.Replace("\\", "\\\\").Replace("'", "\\'");

        /// <summary>
        /// Changes the AllowComputerToTurnOffDevice property of an adapter the way the cmdlet Set-NetAdapterPowerManagement does:
        /// the changed instance is given back to WMI, and the adapter restarts.
        /// </summary>
        private static void SetPowerManagement(string adapterName, bool enable)
        {
            using var session = CimSession.Create(null);

            foreach (var settings in session.QueryInstances(CimNamespace, "WQL", $"SELECT * FROM MSFT_NetAdapterPowerManagementSettingData WHERE Name = '{ToQueryText(adapterName)}'"))
            {
                using (settings)
                {
                    var state = enable ? TurnOffDeviceEnabled : TurnOffDeviceDisabled;
                    var property = settings.CimInstanceProperties["AllowComputerToTurnOffDevice"];

                    // The second adapter of a device already has the value its first adapter was given: the device is not restarted twice
                    if (Convert.ToUInt32(property.Value ?? TurnOffDeviceUnsupported) != state)
                    {
                        property.Value = state;
                        using var changed = session.ModifyInstance(CimNamespace, settings);
                    }
                }
            }
        }

        /// <summary>
        /// Gets physical network adapters, as the cmdlet Get-NetAdapter with the Physical parameter does. An adapter that is "Up" is connected.
        /// </summary>
        private static List<(string Name, string Description, bool IsWiFi, bool IsUp, string DeviceId)> GetPhysicalAdapters()
        {
            var adapters = new List<(string Name, string Description, bool IsWiFi, bool IsUp, string DeviceId)>();
            using var searcher = new ManagementObjectSearcher(CimNamespace, "SELECT Name, InterfaceDescription, InterfaceOperationalStatus, NdisPhysicalMedium, PnPDeviceID FROM MSFT_NetAdapter WHERE ConnectorPresent = TRUE AND Hidden = FALSE");

            foreach (var adapter in searcher.Get().Cast<ManagementBaseObject>())
            {
                using (adapter)
                {
                    if (adapter["Name"] is string { Length: > 0 } name)
                    {
                        adapters.Add((
                            name,
                            adapter["InterfaceDescription"] as string ?? string.Empty,
                            Convert.ToUInt32(adapter["NdisPhysicalMedium"] ?? 0u) == WiFiPhysicalMedium,
                            Convert.ToUInt32(adapter["InterfaceOperationalStatus"] ?? 0u) == OperationalStatusUp,
                            adapter["PnPDeviceID"] as string is { Length: > 0 } deviceId ? deviceId : name));
                    }
                }
            }

            return adapters;
        }

        /// <summary>
        /// Gets names and the AllowComputerToTurnOffDevice property state of the adapters that have this property to manage.
        /// </summary>
        /// <param name="adapters">Network adapters.</param>
        private static List<(string Name, uint State)> GetPowerManagement(IEnumerable<(string Name, string Description, bool IsWiFi, bool IsUp, string DeviceId)> adapters)
        {
            var states = new List<(string Name, uint State)>();

            foreach (var name in adapters.Select(adapter => adapter.Name))
            {
                using var searcher = new ManagementObjectSearcher(CimNamespace, $"SELECT AllowComputerToTurnOffDevice FROM MSFT_NetAdapterPowerManagementSettingData WHERE Name = '{ToQueryText(name)}'");

                foreach (var settings in searcher.Get().Cast<ManagementBaseObject>())
                {
                    using (settings)
                    {
                        var state = Convert.ToUInt32(settings["AllowComputerToTurnOffDevice"] ?? TurnOffDeviceUnsupported);

                        if (state != TurnOffDeviceUnsupported)
                        {
                            states.Add((name, state));
                        }
                    }
                }
            }

            return states;
        }

        private static int CountUpAdapters(List<string> adapterNames)
        {
            try
            {
                return GetPhysicalAdapters().Count(adapter => adapter.IsUp && adapterNames.Contains(adapter.Name));
            }
            catch (Exception)
            {
                // The adapters cannot be enumerated while their drivers are reloading
                return 0;
            }
        }

        // What the cmdlet Get-NetConnectionProfile reads
        private static bool HasInternetAccess()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(CimNamespace, "SELECT IPv4Connectivity, IPv6Connectivity FROM MSFT_NetConnectionProfile");

                return searcher.Get().Cast<ManagementBaseObject>().Any(profile =>
                    Convert.ToUInt32(profile["IPv4Connectivity"] ?? 0u) == InternetConnectivity || Convert.ToUInt32(profile["IPv6Connectivity"] ?? 0u) == InternetConnectivity);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string? GetConnectedSsid()
        {
            // The "SSID" label is not localized. "BSSID" lines are skipped, because they do not start with "SSID"
            var line = RunNetsh("wlan show interfaces")
                .Split('\n')
                .Select(text => text.Trim())
                .FirstOrDefault(text => text.StartsWith("SSID", StringComparison.Ordinal) && text.Contains(':'));

            return line?.Split(':', 2)[1].Trim();
        }

        private static string? GetWlanProfileName(string ssid)
        {
            try
            {
                var profilesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Wlansvc\\Profiles\\Interfaces");

                foreach (var file in Directory.EnumerateFiles(profilesFolder, "*.xml", SearchOption.AllDirectories))
                {
                    var profile = XDocument.Load(file).Root;
                    var profileSsid = profile?.Descendants().FirstOrDefault(element => element.Name.LocalName == "SSID")?.Elements().FirstOrDefault(element => element.Name.LocalName == "name")?.Value;

                    if (string.Equals(profileSsid, ssid, StringComparison.OrdinalIgnoreCase))
                    {
                        return profile!.Elements().FirstOrDefault(element => element.Name.LocalName == "name")?.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                // WLAN profiles cannot be read
                App.Logger.LogSkippedException("Reading the WLAN profiles", ex);
            }

            return null;
        }

        private static string RunNetsh(string arguments)
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "netsh.exe"), arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = GetOemEncoding(),
            };
            _ = process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static Encoding GetOemEncoding()
        {
            try
            {
                // Console programs write their output in the OEM code page
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            }
            catch (Exception)
            {
                return Encoding.Default;
            }
        }

        /// <summary>
        /// Waits until at least one adapter goes down: adapters restart asynchronously and stay "Up" for a few seconds after their settings were changed.
        /// </summary>
        /// <param name="adapterNames">Names of the adapters whose settings were changed.</param>
        private static void WaitForRestart(List<string> adapterNames)
        {
            // Some drivers don't restart the adapter at all, so wait 10 seconds at most
            var timer = Stopwatch.StartNew();
            int upAdapters;

            do
            {
                Thread.Sleep(500);

                // An adapter may disappear while its driver is reloading, so count only adapters that are up
                upAdapters = CountUpAdapters(adapterNames);
            }
            while (upAdapters == adapterNames.Count && timer.Elapsed.TotalSeconds < 10);
        }

        /// <summary>
        /// Waits until Internet access is restored, or until adapters are up if there was no Internet access before. 60 seconds at most.
        /// </summary>
        /// <param name="adapterNames">Names of the adapters whose settings were changed.</param>
        /// <param name="wiFiAdapters">Names of the Wi-Fi adapters among them.</param>
        /// <param name="hadInternet">Whether PC had Internet access before the adapters restart.</param>
        /// <param name="ssid">SSID of the Wi-Fi network that was used, if any.</param>
        /// <param name="profileName">WLAN profile name of that network.</param>
        private static void WaitForConnection(List<string> adapterNames, List<string> wiFiAdapters, bool hadInternet, string? ssid, string? profileName)
        {
            var timer = Stopwatch.StartNew();
            var attempt = 0;

            while (timer.Elapsed.TotalSeconds < 60)
            {
                var isReady = hadInternet ? HasInternetAccess() : CountUpAdapters(adapterNames) == adapterNames.Count;

                if (isReady)
                {
                    break;
                }

                // If Wi-Fi network was used and Wi-Fi adapter is not connected yet, connect to it again
                // Retry every 10 seconds in case the adapter was not ready
                if (ssid is not null && attempt % 5 == 0 && CountUpAdapters(wiFiAdapters) == 0)
                {
                    _ = RunNetsh($"wlan connect name=\"{profileName}\" ssid=\"{ssid}\"");
                }

                Thread.Sleep(2000);
                attempt++;
            }
        }
    }
}
