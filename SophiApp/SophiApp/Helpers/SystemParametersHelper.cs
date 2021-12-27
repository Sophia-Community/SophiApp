using System;
using System.Runtime.InteropServices;

namespace SophiApp.Helpers
{
    internal class SystemParametersHelper
    {
        private const int SPI_GETTHREADLOCALINPUTSETTINGS = 0x104E;
        private const int SPI_SETTHREADLOCALINPUTSETTINGS = 0x104F;
        private const int SPIF_SENDCHANGE = 0x02;
        private const int SPIF_UPDATEINIFILE = 0x01;
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfow

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, [MarshalAs(UnmanagedType.Bool)] out bool pvParam, uint fWinIni);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        internal static bool GetInputSettings()
        {
            _ = SystemParametersInfo(SPI_GETTHREADLOCALINPUTSETTINGS, 0, out bool result, 0);
            return result;
        }

        internal static void SetInputSettings(bool value) => SystemParametersInfo(SPI_SETTHREADLOCALINPUTSETTINGS, 0, Convert.ToByte(value), SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }
}