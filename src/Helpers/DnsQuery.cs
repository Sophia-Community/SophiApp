// <copyright file="DnsQuery.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Asks the DNS servers of one network adapter. An ordinary query goes through all adapters,
    /// so the servers of another adapter may answer instead of the ones that are being checked.
    /// </summary>
    public static class DnsQuery
    {
        private const uint RequestVersion = 1;
        private const uint ResultsVersion = 1;
        private const ushort AddressRecord = 1;
        private const int FreeRecordList = 1;

        // DNS_QUERY_BYPASS_CACHE | DNS_QUERY_NO_HOSTS_FILE | DNS_QUERY_NO_NETBT | DNS_QUERY_NO_MULTICAST
        private const ulong Options = 0x8 | 0x40 | 0x80 | 0x800;

        /// <summary>
        /// Gets a value indicating whether the DNS servers of the adapter answer the query for the address of a name.
        /// </summary>
        /// <param name="name">The name to resolve.</param>
        /// <param name="interfaceIndex">The index of the network adapter.</param>
        public static bool Resolve(string name, int interfaceIndex)
        {
            var request = new DnsQueryRequest
            {
                Version = RequestVersion,
                QueryName = name,
                QueryType = AddressRecord,
                QueryOptions = Options,
                InterfaceIndex = (uint)interfaceIndex,
            };

            var result = new DnsQueryResult { Version = ResultsVersion };
            var status = DnsQueryEx(ref request, ref result, nint.Zero);

            if (result.QueryRecords != nint.Zero)
            {
                DnsRecordListFree(result.QueryRecords, FreeRecordList);
            }

            return status == 0;
        }

        // The exact name is required: the older DnsQueryExW answers that it is not implemented
        [DllImport("dnsapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int DnsQueryEx(ref DnsQueryRequest request, ref DnsQueryResult results, nint cancelHandle);

        [DllImport("dnsapi.dll")]
        private static extern void DnsRecordListFree(nint recordList, int freeType);

        // The fields repeat the structures of Windows: all of them are needed for the size, though not all are read
        #pragma warning disable S4487

        [StructLayout(LayoutKind.Sequential)]
        private struct DnsQueryRequest
        {
            public uint Version;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string QueryName;
            public ushort QueryType;
            public ulong QueryOptions;
            public nint DnsServerList;
            public uint InterfaceIndex;
            public nint QueryCompletionCallback;
            public nint QueryContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DnsQueryResult
        {
            public uint Version;
            public int QueryStatus;
            public ulong QueryOptions;
            public nint QueryRecords;
            public nint Reserved;
        }

        #pragma warning restore S4487
    }
}
