// <copyright file="PowerCapabilities.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// What the power management of the device can do.
    /// </summary>
    public static class PowerCapabilities
    {
        // SYSTEM_POWER_CAPABILITIES: the size of the structure and the place of its SystemS4 field
        private const int StructureSize = 76;
        private const int SystemS4Offset = 6;

        /// <summary>
        /// Gets a value indicating whether the firmware supports hibernation (the S4 sleep state).
        /// </summary>
        public static bool HibernationIsSupported
        {
            get
            {
                var capabilities = new byte[StructureSize];

                // If Windows does not answer, hibernation is not ruled out
                return !GetPwrCapabilities(capabilities) || capabilities[SystemS4Offset] != 0;
            }
        }

        [DllImport("powrprof.dll")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool GetPwrCapabilities([Out] byte[] capabilities);
    }
}
