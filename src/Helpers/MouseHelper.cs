// <copyright file="MouseHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Reads and changes the mouse acceleration, which Windows calls "Enhance pointer precision".
    /// </summary>
    public static class MouseHelper
    {
        // SPI_GETMOUSE and SPI_SETMOUSE: two thresholds and the acceleration
        private const uint GetMouse = 0x0003;
        private const uint SetMouse = 0x0004;

        // SPIF_UPDATEINIFILE | SPIF_SENDCHANGE: the values are saved for the user and used at once
        private const uint SaveAndApply = 0x01 | 0x02;

        /// <summary>
        /// Gets a value indicating whether the mouse acceleration is turned on.
        /// </summary>
        public static bool GetAcceleration()
        {
            var values = new int[3];
            return SystemParametersInfo(GetMouse, 0, values, 0) && values[2] != 0;
        }

        /// <summary>
        /// Turns the mouse acceleration on or off. The change is used at once, without signing out.
        /// </summary>
        /// <param name="enable">True to turn the acceleration on, with the thresholds Windows has by default.</param>
        public static void SetAcceleration(bool enable)
        {
            int[] values = enable ? [6, 10, 1] : [0, 0, 0];

            if (!SystemParametersInfo(SetMouse, 0, values, SaveAndApply))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint action, uint parameter, [In, Out] int[] values, uint flags);
    }
}
