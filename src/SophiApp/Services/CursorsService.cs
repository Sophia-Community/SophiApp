// <copyright file="CursorsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <inheritdoc/>
    public class CursorsService : ICursorsService
    {
        private readonly IHttpService httpService;
        private readonly IProcessService processService;
        private readonly string jepriCursorsZip;
        private readonly string jepriDarkUrl = "https://github.com/farag2/Sophia-Script-for-Windows/raw/master/Misc/dark_new.zip"; // TODO: Change repo
        private readonly string jepriLightUrl = "https://github.com/farag2/Sophia-Script-for-Windows/raw/master/Misc/light_new.zip"; // TODO: Change repo
        private readonly string tarExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "tar.exe");
        private readonly string jepriDarkCursorsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Cursors\\W11 Cursor Dark Free");
        private readonly string jepriLightCursorsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Cursors\\W11 Cursor Light Free");

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorsService"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="processService">A service for working with Windows <see cref="Process"/> API.</param>
        public CursorsService(IHttpService httpService, IProcessService processService)
        {
            var downloadFolderPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders";
            var downloadFolder = Registry.CurrentUser.OpenSubKey(downloadFolderPath)?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            jepriCursorsZip = $"{downloadFolder}\\JepriCreationsW11CursorsFree.zip";
            this.httpService = httpService;
            this.processService = processService;
        }

        /// <inheritdoc/>
        public void ReloadCursors() => _ = SystemParametersInfo(0x0057, 0, 0, 0);

        /// <inheritdoc/>
        public void SetDefaultCursors()
        {
            var cursorsRegistryPath = "Control Panel\\Cursors";
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue(string.Empty, string.Empty, RegistryValueKind.String);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("AppStarting", "%SystemRoot%\\cursors\\aero_working.ani", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Arrow", "%SystemRoot%\\cursors\\aero_arrow.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Crosshair", string.Empty, RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Hand", "%SystemRoot%\\cursors\\aero_link.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Help", "%SystemRoot%\\cursors\\aero_helpsel.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("IBeam", string.Empty, RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("No", "%SystemRoot%\\cursors\\aero_unavail.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("NWPen", "%SystemRoot%\\cursors\\aero_pen.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Person", "%SystemRoot%\\cursors\\aero_person.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Pin", "%SystemRoot%\\cursors\\aero_pin.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Scheme Source", 2, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeAll", "%SystemRoot%\\cursors\\aero_move.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeNESW", "%SystemRoot%\\cursors\\aero_nesw.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeNS", "%SystemRoot%\\cursors\\aero_ns.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeNWSE", "%SystemRoot%\\cursors\\aero_nwse.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeWE", "%SystemRoot%\\cursors\\aero_ew.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("UpArrow", "%SystemRoot%\\cursors\\aero_up.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Wait", "%SystemRoot%\\cursors\\aero_up.cur", RegistryValueKind.ExpandString);
        }

        /// <inheritdoc/>
        public void SetJepriCreationsDarkCursors() => SetJepriCursors(jepriDarkUrl, jepriDarkCursorsFolder, "W11 Cursor Dark Free by Jepri Creations");

        /// <inheritdoc/>
        public void SetJepriCreationsLightCursors() => SetJepriCursors(jepriLightUrl, jepriLightCursorsFolder, "W11 Cursor Light Free by Jepri Creations");

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        private void SetJepriCursors(string downloadUrl, string cursorsFolder, string schemeName)
        {
            var cursorsRegistryPath = "Control Panel\\Cursors";
            var cursorsFolderPath = cursorsFolder.Replace(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "%SystemRoot%");
            var schemeValue = $"{cursorsFolderPath}\\arrow.cur,{cursorsFolderPath}\\help.cur,{cursorsFolderPath}\\appstarting.ani,{cursorsFolderPath}\\wait.ani,{cursorsFolderPath}\\crosshair.cur,{cursorsFolderPath}\\sizens.cur,{cursorsFolderPath}\\nwpen.cur,{cursorsFolderPath}\\no.cur,{cursorsFolderPath}\\sizens.cur,{cursorsFolderPath}\\sizewe.cur,{cursorsFolderPath}\\sizenwse.cur,{cursorsFolderPath}\\sizenesw.cur,{cursorsFolderPath}\\sizeall.cur,{cursorsFolderPath}\\uparrow.cur,{cursorsFolderPath}\\hand.cur,{cursorsFolderPath}\\person.cur,{cursorsFolderPath}\\pin.cur";
            httpService.DownloadFile(downloadUrl, jepriCursorsZip);
            Directory.CreateDirectory(cursorsFolder);
            _ = processService.WaitForExit(tarExe, $"-xvf \"{jepriCursorsZip}\" -C \"{cursorsFolder}\"");
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue(string.Empty, schemeName, RegistryValueKind.String);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("AppStarting", $"{cursorsFolderPath}\\appstarting.ani", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Arrow", $"{cursorsFolderPath}\\arrow.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Crosshair", $"{cursorsFolderPath}\\crosshair.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Hand", $"{cursorsFolderPath}\\hand.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Help", $"{cursorsFolderPath}\\help.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("IBeam", $"{cursorsFolderPath}\\ibeam.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("No", $"{cursorsFolderPath}\\no.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("NWPen", $"{cursorsFolderPath}\\nwpen.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Person", $"{cursorsFolderPath}\\person.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Pin", $"{cursorsFolderPath}\\pin.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Scheme Source", 1, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeAll", $"{cursorsFolderPath}\\sizeall.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeNESW", $"{cursorsFolderPath}\\sizenesw.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeNS", $"{cursorsFolderPath}\\sizens.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeNWSE", $"{cursorsFolderPath}\\sizenwse.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("SizeWE", $"{cursorsFolderPath}\\sizewe.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("UpArrow", $"{cursorsFolderPath}\\uparrow.cur", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenSubKey(cursorsRegistryPath, true)?.SetValue("Wait", $"{cursorsFolderPath}\\wait.ani", RegistryValueKind.ExpandString);
            Registry.CurrentUser.OpenOrCreateSubKey(Path.Combine(cursorsRegistryPath, "Schemes")).SetValue(schemeName, schemeValue, RegistryValueKind.String);
            File.Delete(jepriCursorsZip);
        }
    }
}
