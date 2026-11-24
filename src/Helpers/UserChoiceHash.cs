// <copyright file="UserChoiceHash.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The hash Windows keeps next to the default app of an extension or a protocol.
    /// Windows takes a default app without the right hash for a change made behind the user's back and drops it.
    /// </summary>
    public static class UserChoiceHash
    {
        // The text Windows adds to what it hashes. It is kept in shell32.dll
        private const string UserExperience = "User Choice set via Windows User Experience {D18B6DD5-6124-4341-9318-804003BAFA0B}";

        // A minute in the units of the time of a registry key: 100 nanoseconds
        private const long Minute = 600000000L;

        /// <summary>
        /// Gets the hash of a default app. The hash includes the minute the key of the app is written in,
        /// so the key has to be written within that minute.
        /// </summary>
        /// <param name="extension">The extension with the dot, or the name of the protocol.</param>
        /// <param name="userSid">The security identifier of the user.</param>
        /// <param name="progId">The ProgId of the default app.</param>
        /// <param name="minute">The minute the key is written in: <see cref="GetCurrentMinute"/> or <see cref="GetKeyMinute"/>.</param>
        public static string Get(string extension, string userSid, string progId, long minute)
        {
            var text = (extension + userSid + progId + minute.ToString("x16") + UserExperience).ToLowerInvariant();

            // UTF-16 with the terminating null character
            var data = Encoding.Unicode.GetBytes(text + "\0");

            #pragma warning disable CA5351, S4790 // The algorithm is the one Windows checks the hash with
            var md5 = MD5.HashData(data);
            #pragma warning restore CA5351, S4790

            // The number of 32-bit words, rounded down to an even one
            var length = (data.Length >> 2) & ~1;

            unchecked
            {
                var c0 = BitConverter.ToUInt32(md5, 0) | 1;
                var c1 = BitConverter.ToUInt32(md5, 4) | 1;
                var d0 = c0 + 0x69FB0000;
                var d1 = c1 + 0x13DB0000;

                // Two mixes of the same data: a* is the first one, b* is the second one
                uint a1 = 0, a2 = 0, b1 = 0, b2 = 0;

                for (var offset = 0; offset < length * 4; offset += 8)
                {
                    var x0 = BitConverter.ToUInt32(data, offset);
                    var x1 = BitConverter.ToUInt32(data, offset + 4);

                    var n = x0 + a1;
                    var t = (n * d0) - (0x10FA9605 * (n >> 16));
                    var v1 = (0x79F8A395 * t) + (0x689B6B9F * (t >> 16));
                    var v2 = (0xEA970001 * v1) - (0x3C101569 * (v1 >> 16));
                    var v3 = x1 + v2;
                    var v4 = (v3 * d1) - (0x3CE8EC25 * (v3 >> 16));
                    var v5 = (0x59C3AF2D * v4) - (0x2232E0F1 * (v4 >> 16));
                    a1 = (0x1EC90001 * v5) + (0x35BD1EC9 * (v5 >> 16));
                    a2 += a1 + v2;

                    n = (x0 + b1) * c0;
                    n = (0xB1110000 * n) - (0x30674EEF * (n >> 16));
                    v1 = (0x5B9F0000 * n) - (0x78F7A461 * (n >> 16));
                    t = (0x12CEB96D * (v1 >> 16)) - (0x46930000 * v1);
                    v2 = (0x1D830000 * t) + (0x257E1D83 * (t >> 16));
                    v3 = c1 * (x1 + v2);
                    v4 = (0x16F50000 * v3) - (0x5D8BE90B * (v3 >> 16));
                    t = (0x96FF0000 * v4) - (0x2C7C6901 * (v4 >> 16));
                    v5 = (0x2B890000 * t) + (0x7C932B89 * (t >> 16));
                    b1 = (0x9F690000 * v5) - (0x405B6097 * (v5 >> 16));
                    b2 += b1 + v2;
                }

                var hash = new byte[8];
                BitConverter.GetBytes(a1 ^ b1).CopyTo(hash, 0);
                BitConverter.GetBytes(a2 ^ b2).CopyTo(hash, 4);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Gets the current time cut to whole minutes, in the units of the time of a registry key.
        /// </summary>
        public static long GetCurrentMinute()
        {
            var time = DateTime.UtcNow.ToFileTimeUtc();
            return time - (time % Minute);
        }

        /// <summary>
        /// Gets the time a key of the current user was last written at, cut to whole minutes. 0 if there is no such key.
        /// </summary>
        /// <param name="subKey">The key of the current user.</param>
        public static long GetKeyMinute(string subKey)
        {
            using var key = Registry.CurrentUser.OpenSubKey(subKey);
            return key is not null && RegQueryInfoKey(key.Handle, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, out var time) == 0 ? time - (time % Minute) : 0;
        }

        [DllImport("advapi32.dll", EntryPoint = "RegQueryInfoKeyW")]
        private static extern int RegQueryInfoKey(SafeRegistryHandle key, nint keyClass, nint classLength, nint reserved, nint subKeys, nint maxSubKeyLength, nint maxClassLength, nint values, nint maxValueNameLength, nint maxValueLength, nint securityDescriptor, out long lastWriteTime);
    }
}
