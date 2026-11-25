// <copyright file="InitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.ViewModels;
using SophiApp.Views;
using System.Threading.Tasks;

/// <inheritdoc/>
public class InitializeService : IInitializeService
{
    private readonly ICommonDataService dataService;
    private readonly IDisplayService displayService;
    private readonly ISettingsService settingsService;
    private readonly IThemesService themesService;
    private readonly ShellViewModel viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeService"/> class.
    /// </summary>
    /// <param name="commonDataService">A service for working with common app data.</param>
    /// <param name="displayService">A service for working with display API.</param>
    /// <param name="settingsService">A service for working with app settings.</param>
    /// <param name="themesService">A service for working with app themes.</param>
    /// <param name="viewModel">Implements the <see cref="ShellViewModel"/> class.</param>
    public InitializeService(
        ICommonDataService commonDataService,
        IDisplayService displayService,
        ISettingsService settingsService,
        IThemesService themesService,
        ShellViewModel viewModel)
    {
        dataService = commonDataService;
        this.displayService = displayService;
        this.settingsService = settingsService;
        this.themesService = themesService;
        this.viewModel = viewModel;
    }

    /// <inheritdoc/>
    public async Task InitializeServicesDataAsync(object args)
    {
        settingsService.Initialize();
        await themesService.InitializeAsync();
        await themesService.SetRequestedThemeAsync();
        await dataService.InitializeAsync();
    }

    /// <inheritdoc/>
    public async Task InitializeMainWindowAsync()
    {
        App.MainWindow.Title = dataService.GetFullName();
        var windowState = await settingsService.ReadAppWindowStateAsync();
        var windowPosition = await settingsService.ReadAppWindowPositionAsync();
        var windowHeight = await settingsService.ReadAppWindowHeightAsync();
        var windowWidth = await settingsService.ReadAppWindowWidthAsync();
        var displayArea = await displayService.GetDisplayAreaAsync();

        // The size of the window is counted in the units of the display scale, the position and the work area of the screen in pixels.
        // On a laptop the work area can be smaller than the usual size of the window, and even than its minimal size
        var scale = App.MainWindow.GetDpiForWindow() / 96d;
        var workArea = displayArea?.WorkArea ?? default;
        var hasWorkArea = workArea.Width > 0 && workArea.Height > 0;
        var availableHeight = hasWorkArea ? workArea.Height / scale : double.MaxValue;
        var availableWidth = hasWorkArea ? workArea.Width / scale : double.MaxValue;
        App.MainWindow.MinHeight = Math.Min(settingsService.AppWindowMinHeight, availableHeight);
        App.MainWindow.MinWidth = Math.Min(settingsService.AppWindowMinWidth, availableWidth);

        // The saved place is used only if the whole window is on the screen there
        var hasCorrectPosition = windowPosition.X > 0 && windowPosition.Y > 0;
        var isOnScreen = hasWorkArea
            && windowPosition.X >= workArea.X
            && windowPosition.Y >= workArea.Y
            && windowPosition.X + (windowWidth * scale) <= workArea.X + workArea.Width
            && windowPosition.Y + (windowHeight * scale) <= workArea.Y + workArea.Height;

        if (windowState == WindowState.Maximized)
        {
            App.MainWindow.WindowState = WindowState.Maximized;
        }
        else if (hasCorrectPosition && isOnScreen)
        {
            App.MainWindow.MoveAndResize(x: windowPosition.X, y: windowPosition.Y, width: windowWidth, height: windowHeight);
        }
        else if (hasWorkArea)
        {
            // In the middle of the work area, so the taskbar is not covered
            var height = Math.Min(settingsService.AppWindowDefaultHeight, availableHeight);
            var width = Math.Min(settingsService.AppWindowDefaultWidth, availableWidth);
            var left = workArea.X + ((workArea.Width - (width * scale)) / 2);
            var top = workArea.Y + ((workArea.Height - (height * scale)) / 2);
            App.MainWindow.MoveAndResize(x: left, y: top, width: width, height: height);
        }
        else
        {
            App.MainWindow.Height = settingsService.AppWindowDefaultHeight;
            App.MainWindow.Width = settingsService.AppWindowDefaultWidth;
            App.MainWindow.CenterOnScreen();
        }

        // The menu is created in a window that already has its size. When a narrow window got its size after the menu,
        // the menu sometimes was not drawn until the window was made wider
        App.MainWindow.Content = App.MainWindow.Content is null ? App.GetService<ShellPage>() : new Frame();
        await themesService.SetRequestedThemeAsync();
        TitleBarHelper.ApplySystemThemeToCaptionButtons();
        App.MainWindow.Activate();
        await viewModel.ExecuteAsync();
    }
}
