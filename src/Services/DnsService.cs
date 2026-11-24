// <copyright file="DnsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Management.Infrastructure;
    using Microsoft.Management.Infrastructure.Options;
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
        private const string EncryptedServersKeyV6 = "DohInterfaceSettings\\Doh6";

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

        private readonly IInstrumentationService instrumentationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsService"/> class.
        /// </summary>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        public DnsService(IInstrumentationService instrumentationService)
        {
            this.instrumentationService = instrumentationService;
        }

        /// <inheritdoc/>
        public int GetProvider()
        {
            // Every adapter has to be set to the same provider
            var providers = GetAdaptersInUse().Select(GetProvider).Distinct().ToList();

            if (providers.Count == 1)
            {
                return providers[0];
            }

            return providers.Contains(ThirdPartyState.Id) ? ThirdPartyState.Id : 0;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> GetEncryptedServers()
            => GetAdaptersInUse().SelectMany(adapter => GetServers(adapter).Where(server => IsEncrypted(adapter.Id, server))).Select(server => server.ToString()).Distinct().ToList();

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
                SetServers(GetIndex(adapter), primary, secondary);

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

        // The DNS servers of an adapter, of both IP versions
        private static List<IPAddress> GetServers(NetworkInterface adapter)
            => [.. adapter.GetIPProperties().DnsAddresses.Where(address => address.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6)];

        /// <summary>
        /// Gets the number of the provider an adapter is set to, 0 if it does not encrypt the queries,
        /// or <see cref="ThirdPartyState.Id"/> if it encrypts them in a way the function does not set.
        /// </summary>
        private static int GetProvider(NetworkInterface adapter)
        {
            var allServers = GetServers(adapter);

            // The function sets IPv4 servers only
            var servers = allServers.Where(server => server.AddressFamily == AddressFamily.InterNetwork).Select(server => server.ToString()).ToList();
            var encrypted = servers.Where(server => IsEncrypted(adapter.Id, server)).ToList();

            if (encrypted.Count == 0)
            {
                // The queries can be encrypted for IPv6 servers alone
                return allServers.Exists(server => IsEncrypted(adapter.Id, server)) ? ThirdPartyState.Id : 0;
            }

            // All servers of the adapter are encrypted and belong to one provider. One of its two servers is enough: the second one may not be set
            var number = Array.FindIndex(Providers, provider => servers.TrueForAll(server => server == provider.Primary || server == provider.Secondary)) + 1;
            return number > 0 && encrypted.Count == servers.Count ? number : ThirdPartyState.Id;
        }

        private static string GetServerKey(string adapterId, string server) => $"{AdaptersKey}\\{adapterId}\\{EncryptedServersKey}\\{server}";

        private static bool IsEncrypted(string adapterId, string server) => Registry.LocalMachine.SubKeyExists(GetServerKey(adapterId, server));

        // The servers of IPv6 are kept in a key of their own
        private static bool IsEncrypted(string adapterId, IPAddress server) => server.AddressFamily == AddressFamily.InterNetworkV6
            ? Registry.LocalMachine.SubKeyExists($"{AdaptersKey}\\{adapterId}\\{EncryptedServersKeyV6}\\{server}")
            : IsEncrypted(adapterId, server.ToString());

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

            foreach (var serversKey in (adapters?.GetSubKeyNames() ?? []).SelectMany(adapterId => new[] { $"{adapterId}\\{EncryptedServersKey}", $"{adapterId}\\{EncryptedServersKeyV6}" }))
            {
                using var servers = adapters!.OpenSubKey(serversKey, writable: true);

                foreach (var server in servers?.GetSubKeyNames() ?? [])
                {
                    servers!.DeleteSubKeyTree(server, throwOnMissingSubKey: false);
                }
            }
        }

        /// <summary>
        /// Returns the adapters to the DNS servers the network gives and turns DNS-over-HTTPS off.
        /// </summary>
        private static void Reset(List<NetworkInterface> adapters)
        {
            foreach (var adapter in adapters)
            {
                SetServers(GetIndex(adapter));
            }

            RemoveEncryption();
        }

        /// <summary>
        /// Sets the DNS servers of an adapter the way the cmdlet Set-DnsClientServerAddress does: the servers are an option
        /// of the change of the WMI instance, not its property. Without servers the adapter returns to the ones the network gives.
        /// </summary>
        private static void SetServers(int interfaceIndex, params string[] servers)
        {
            try
            {
                using var session = CimSession.Create(null);
                using var options = new CimOperationOptions();

                if (servers.Length > 0)
                {
                    options.SetCustomOption("ServerAddresses", servers, Microsoft.Management.Infrastructure.CimType.StringArray, mustComply: false);
                }
                else
                {
                    options.SetCustomOption("ResetServerAddresses", true, Microsoft.Management.Infrastructure.CimType.Boolean, mustComply: false);
                }

                foreach (var address in session.QueryInstances("root/StandardCimv2", "WQL", $"SELECT * FROM MSFT_DNSClientServerAddress WHERE InterfaceIndex = {interfaceIndex}"))
                {
                    using (address)
                    {
                        using var changed = session.ModifyInstance("root/StandardCimv2", address, options);
                    }
                }
            }
            catch (CimException ex)
            {
                // The check that follows finds servers that were not set
                App.Logger.LogSkippedException($"Setting the DNS servers of the adapter {interfaceIndex}", ex);
            }
        }

        private void Refresh()
        {
            // What the cmdlets Clear-DnsClientCache and Register-DnsClient call
            _ = instrumentationService.TryInvokeStaticMethod("root\\StandardCimv2", "MSFT_DNSClientCache", "Clear");
            _ = instrumentationService.TryInvokeStaticMethod("root\\StandardCimv2", "MSFT_DNSClient", "Register");
        }
    }
}
