// <copyright file="CursorsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Runtime.InteropServices;

    /// <inheritdoc/>
    public class CursorsService : ICursorsService
    {
        private readonly IHttpService httpService;
        private readonly IProcessService processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorsService"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        public CursorsService(IHttpService httpService, IProcessService processService)
        {
            this.httpService = httpService;
            this.processService = processService;
        }

        /// <inheritdoc/>
        public void ReloadCursors() => SystemParametersInfo(0x0057, 0, 0, 0);

        /// <inheritdoc/>
        public void SetDefaultCursors()
        {
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue(string.Empty, string.Empty, RegistryValueKind.String);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("AppStarting", "%SystemRoot%\\cursors\\aero_working.ani", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Arrow", "%SystemRoot%\\cursors\\aero_arrow.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Crosshair", string.Empty, RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Hand", "%SystemRoot%\\cursors\\aero_link.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Help", "%SystemRoot%\\cursors\\aero_helpsel.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("IBeam", string.Empty, RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("No", "%SystemRoot%\\cursors\\aero_unavail.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("NWPen", "%SystemRoot%\\cursors\\aero_pen.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Person", "%SystemRoot%\\cursors\\aero_person.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Pin", "%SystemRoot%\\cursors\\aero_pin.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Scheme Source", 2, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeAll", "%SystemRoot%\\cursors\\aero_move.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeNESW", "%SystemRoot%\\cursors\\aero_nesw.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeNS", "%SystemRoot%\\cursors\\aero_ns.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeNWSE", "%SystemRoot%\\cursors\\aero_nwse.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeWE", "%SystemRoot%\\cursors\\aero_ew.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("UpArrow", "%SystemRoot%\\cursors\\aero_up.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Wait", "%SystemRoot%\\cursors\\aero_busy.ani", RegistryValueKind.ExpandString);
        }

        /// <inheritdoc/>
        public void SetJepriCreationsCursors(JepriCursorsTheme theme)
        {
            var cursorsZip = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Cursors", "Windows11Cursors.zip");
            var cursorsTheme = theme == JepriCursorsTheme.Light ? "W11 Cursor Light Free by Jepri Creations" : "W11 Cursor Dark Free by Jepri Creations";
            var extractPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), theme == JepriCursorsTheme.Light ? "Cursors\\W11 Cursor Light Free" : "Cursors\\W11 Cursor Dark Free");
            var extractArguments = $"-xvf \"{cursorsZip}\" -C \"{extractPath}\" --strip-components=1 {(theme == JepriCursorsTheme.Light ? "light" : "dark")}/";
            var systemRootPath = extractPath.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "%SystemRoot%");
            _ = Directory.CreateDirectory(extractPath);
            httpService.DownloadFile("https://raw.githubusercontent.com/farag2/Sophia-Script-for-Windows/refs/heads/main/Cursors/w11-cursor-concept-free.zip", cursorsZip);

            // Extract archive
            _ = processService.WaitForExit(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "tar.exe"), extractArguments);

            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue(string.Empty, cursorsTheme, RegistryValueKind.String);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("AppStarting", $"{systemRootPath}\\appstarting.ani", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Arrow", $"{systemRootPath}\\arrow.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Crosshair", $"{systemRootPath}\\crosshair.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Hand", $"{systemRootPath}\\hand.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Help", $"{systemRootPath}\\help.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("IBeam", $"{systemRootPath}\\ibeam.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("No", $"{systemRootPath}\\no.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("NWPen", $"{systemRootPath}\\nwpen.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Person", $"{systemRootPath}\\person.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Pin", $"{systemRootPath}\\pin.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Scheme Source", 1, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeAll", $"{systemRootPath}\\sizeall.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeNESW", $"{systemRootPath}\\sizenesw.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeNS", $"{systemRootPath}\\sizens.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeNWSE", $"{systemRootPath}\\sizenwse.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("SizeWE", $"{systemRootPath}\\sizewe.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("UpArrow", $"{systemRootPath}\\uparrow.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors", true)?.SetValue("Wait", $"{systemRootPath}\\wait.ani", RegistryValueKind.ExpandString);

            var cursorScheme = string.Join(',', new List<string>()
            {
                $"{systemRootPath}\\arrow.cur",
                $"{systemRootPath}\\help.cur",
                $"{systemRootPath}\\appstarting.ani",
                $"{systemRootPath}\\wait.ani",
                $"{systemRootPath}\\crosshair.cur",
                $"{systemRootPath}\\sizens.cur",
                $"{systemRootPath}\\nwpen.cur",
                $"{systemRootPath}\\no.cur",
                $"{systemRootPath}\\sizens.cur",
                $"{systemRootPath}\\sizewe.cur",
                $"{systemRootPath}\\sizenwse.cur",
                $"{systemRootPath}\\sizenesw.cur",
                $"{systemRootPath}\\sizeall.cur",
                $"{systemRootPath}\\uparrow.cur",
                $"{systemRootPath}\\hand.cur",
                $"{systemRootPath}\\person.cur",
                $"{systemRootPath}\\pin.cur",
            });

            Registry.CurrentUser.OpenOrCreateSubKey(Path.Combine("Control Panel\\Cursors", "Schemes")).SetValue(cursorsTheme, cursorScheme, RegistryValueKind.String);
            File.Delete(cursorsZip);
            File.Delete(Path.Combine(extractPath, "Install.inf"));
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
    }
}
