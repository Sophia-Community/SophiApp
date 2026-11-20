// <copyright file="DnsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    /// <inheritdoc/>
    public class DnsService : IDnsService
    {
        // DNS settings of network adapters. The subkey of an adapter is its GUID
        private const string AdaptersKey = "SYSTEM\\CurrentControlSet\\Services\\Dnscache\\InterfaceSpecificParameters";

        // DNS servers of an adapter that are queried over HTTPS. The subkey of a server is its address
        private const string EncryptedServersKey = "DohInterfaceSettings\\Doh";

        // DohFlags: the address of the DNS-over-HTTPS service is set manually
        private const long ManualTemplate = 2;

        // The name Windows itself resolves to see whether there is an Internet connection
        private const string ProbeName = "dns.msftncsi.com";
        private const int Attempts = 2;

        // InterfaceOperationalStatus of MSFT_NetAdapter: the adapter status is "Up"
        private const uint OperationalStatusUp = 1;

        #pragma warning disable S1313 // The addresses of public DNS servers never change, and they are what the function sets

        // The position in the list is the number of the provider, starting from 1
        private static readonly (string Primary, string Secondary, string Template)[] Providers =
        [
            // Cloudflare DNS: https://developers.cloudflare.com/1.1.1.1/setup/windows/
            ("1.1.1.1", "1.0.0.1", "https://cloudflare-dns.com/dns-query"),

            // Google Public DNS: https://developers.google.com/speed/public-dns/docs/using
            ("8.8.8.8", "8.8.4.4", "https://dns.google/dns-query"),

            // Quad9 DNS: https://quad9.net/service/service-addresses-and-features/
            ("9.9.9.9", "149.112.112.112", "https://dns.quad9.net/dns-query"),

            // AdGuard DNS: https://adguard-dns.io/public-dns.html
            ("94.140.14.14", "94.140.14.15", "https://dns.adguard-dns.com/dns-query"),

            // OpenDNS: https://www.cisco.com/c/en/us/support/docs/security/umbrella/224705-configure-dns-over-https-doh-with.html
            ("208.67.222.222", "208.67.220.220", "https://doh.umbrella.com/dns-query"),
        ];

        #pragma warning restore S1313

        private readonly IPowerShellService powerShellService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsService"/> class.
        /// </summary>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        public DnsService(IPowerShellService powerShellService)
        {
            this.powerShellService = powerShellService;
        }

        /// <inheritdoc/>
        public int GetProvider()
        {
            // Every adapter has to be set to the same provider
            var providers = GetAdaptersInUse().Select(GetProvider).Distinct().ToList();
            return providers.Count == 1 ? providers[0] : 0;
        }

        /// <inheritdoc/>
        public void SetProvider(int provider)
        {
            var adapters = GetAdaptersInUse();

            // Reset DNS servers first
            Reset(adapters);

            if (provider == 0)
            {
                Refresh();
                return;
            }

            // The adapters whose default servers answer. If none does, there is no Internet connection:
            // the new servers cannot be checked, and there is nothing to return to, so they are set without a check
            var connected = adapters.Where(Answers).ToList();
            var (primary, secondary, template) = Providers[provider - 1];

            foreach (var adapter in adapters)
            {
                // Set primary and secondary DNS servers
                _ = powerShellService.Invoke($"Set-DnsClientServerAddress -InterfaceIndex {GetIndex(adapter)} -ServerAddresses '{primary}', '{secondary}'");

                // Enable DNS over HTTPS for both servers
                SetEncryption(adapter.Id, primary, template);
                SetEncryption(adapter.Id, secondary, template);
            }

            Refresh();

            // The default servers answered, so the new ones must answer too. The second attempt saves from a return because of one lost query
            if (connected.Exists(adapter => !Enumerable.Range(0, Attempts).Any(_ => Answers(adapter))))
            {
                // The provider is unavailable, for example it has left the country of the user
                Reset(adapters);
                Refresh();
                throw new CustomizationException(exceptionMessage: $"The DNS-over-HTTPS servers {primary} and {secondary} are not responding", resourceKey: "UIModel_Exception_DnsServerNotResponding")
                {
                    Arguments = [$"UIModel_DNSoverHTTPS_Title_{provider}".GetLocalized()],
                };
            }
        }

        /// <summary>
        /// Gets the adapters the computer is connected to a network with: the physical ones that are up and have a gateway.
        /// A VPN tunnel is not among them, even when the route to the Internet goes through it: its DNS servers belong to the VPN app,
        /// and the user sees and changes in Windows Settings the servers of the real connection.
        /// </summary>
        private static List<NetworkInterface> GetAdaptersInUse()
        {
            var adapters = new List<NetworkInterface>();

            try
            {
                var physicalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using var searcher = new ManagementObjectSearcher("root/StandardCimv2", "SELECT InterfaceGuid, InterfaceOperationalStatus FROM MSFT_NetAdapter WHERE ConnectorPresent = TRUE AND Hidden = FALSE");

                foreach (var adapter in searcher.Get().Cast<ManagementBaseObject>())
                {
                    if (Convert.ToUInt32(adapter["InterfaceOperationalStatus"] ?? 0u) == OperationalStatusUp && adapter["InterfaceGuid"] is string id)
                    {
                        _ = physicalIds.Add(id);
                    }
                }

                adapters.AddRange(NetworkInterface.GetAllNetworkInterfaces().Where(adapter => physicalIds.Contains(adapter.Id) && HasGateway(adapter)));
            }
            catch (Exception ex) when (ex is ManagementException || ex is System.Runtime.InteropServices.COMException || ex is NetworkInformationException)
            {
                // The adapters cannot be read: it is the same for the user as no connection
                App.Logger.LogSkippedException("Reading the network adapters", ex);
            }

            return adapters.Count > 0
                ? adapters
                : throw new CustomizationException(exceptionMessage: "The network adapter currently in use was not found", resourceKey: "UIModel_Exception_ActiveNetworkAdapterNotFound");
        }

        private static bool Answers(NetworkInterface adapter) => DnsQuery.Resolve(ProbeName, GetIndex(adapter));

        private static bool HasGateway(NetworkInterface adapter)
            => adapter.GetIPProperties().GatewayAddresses.Any(gateway => gateway.Address.AddressFamily == AddressFamily.InterNetwork && !gateway.Address.Equals(IPAddress.Any));

        private static int GetIndex(NetworkInterface adapter) => adapter.GetIPProperties().GetIPv4Properties().Index;

        /// <summary>
        /// Gets the number of the provider an adapter is set to, or 0.
        /// </summary>
        private static int GetProvider(NetworkInterface adapter)
        {
            var servers = adapter.GetIPProperties().DnsAddresses
                .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
                .Select(address => address.ToString())
                .ToList();

            // The servers were not set, or the queries to some of them are not encrypted
            if (servers.Count == 0 || !servers.TrueForAll(server => IsEncrypted(adapter.Id, server)))
            {
                return 0;
            }

            // All servers of the adapter belong to one provider. One of its two servers is enough: the second one may not be set
            return Array.FindIndex(Providers, provider => servers.TrueForAll(server => server == provider.Primary || server == provider.Secondary)) + 1;
        }

        private static string GetServerKey(string adapterId, string server) => $"{AdaptersKey}\\{adapterId}\\{EncryptedServersKey}\\{server}";

        private static bool IsEncrypted(string adapterId, string server) => Registry.LocalMachine.SubKeyExists(GetServerKey(adapterId, server));

        private static void SetEncryption(string adapterId, string server, string template)
        {
            Registry.LocalMachine.CreateKeyAndSetValue(GetServerKey(adapterId, server), "DohFlags", ManualTemplate, RegistryValueKind.QWord);
            Registry.LocalMachine.CreateKeyAndSetValue(GetServerKey(adapterId, server), "DohTemplate", template, RegistryValueKind.String);
        }

        /// <summary>
        /// Turns DNS-over-HTTPS off for all servers of all adapters.
        /// </summary>
        private static void RemoveEncryption()
        {
            using var adapters = Registry.LocalMachine.OpenSubKey(AdaptersKey);

            foreach (var adapterId in adapters?.GetSubKeyNames() ?? [])
            {
                using var servers = adapters!.OpenSubKey($"{adapterId}\\{EncryptedServersKey}", writable: true);

                foreach (var server in servers?.GetSubKeyNames() ?? [])
                {
                    servers!.DeleteSubKeyTree(server, throwOnMissingSubKey: false);
                }
            }
        }

        /// <summary>
        /// Returns the adapters to the DNS servers the network gives and turns DNS-over-HTTPS off.
        /// </summary>
        private void Reset(List<NetworkInterface> adapters)
        {
            foreach (var adapter in adapters)
            {
                _ = powerShellService.Invoke($"Set-DnsClientServerAddress -InterfaceIndex {GetIndex(adapter)} -ResetServerAddresses");
            }

            RemoveEncryption();
        }

        private void Refresh()
        {
            _ = powerShellService.Invoke("Clear-DnsClientCache");
            _ = powerShellService.Invoke("Register-DnsClient");
        }
    }
}
