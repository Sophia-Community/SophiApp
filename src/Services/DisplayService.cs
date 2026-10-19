// <copyright file="DisplayService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    using SophiApp.Contracts.Services;
    using System.Threading.Tasks;
    using WinRT.Interop;

    /// <inheritdoc/>
    public class DisplayService : IDisplayService
    {
        /// <inheritdoc/>
        public async Task<DisplayArea?> GetDisplayAreaAsync()
        {
            return await Task.Run(() =>
            {
                var handler = WindowNative.GetWindowHandle(App.MainWindow);
                var windowId = Win32Interop.GetWindowIdFromWindow(handler);
                return DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            });
        }
    }
}
