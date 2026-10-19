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
    private readonly IRequirementsService requirementsService;
    private readonly ISettingsService settingsService;
    private readonly IThemesService themesService;
    private readonly ShellViewModel viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeService"/> class.
    /// </summary>
    /// <param name="commonDataService">A service for working with common app data.</param>
    /// <param name="displayService">A service for working with display API.</param>
    /// <param name="requirementsService">A service for working with app requirements.</param>
    /// <param name="settingsService">A service for working with app settings.</param>
    /// <param name="themesService">A service for working with app themes.</param>
    /// <param name="viewModel">Implements the <see cref="ShellViewModel"/> class.</param>
    public InitializeService(
        ICommonDataService commonDataService,
        IDisplayService displayService,
        IRequirementsService requirementsService,
        ISettingsService settingsService,
        IThemesService themesService,
        ShellViewModel viewModel)
    {
        dataService = commonDataService;
        this.displayService = displayService;
        this.requirementsService = requirementsService;
        this.settingsService = settingsService;
        this.themesService = themesService;
        this.viewModel = viewModel;
    }

    /// <inheritdoc/>
    public async Task InitializeServicesDataAsync(object args)
    {
        settingsService.Initialize();
        requirementsService.Initialize();
        await viewModel.FontOptions.InitializeAsync();
        await themesService.InitializeAsync();
        await themesService.SetRequestedThemeAsync();
        await dataService.InitializeAsync();
        viewModel.LogPageVisibility = await settingsService.ReadLogPageVisibilityAsync();
    }

    /// <inheritdoc/>
    public async Task InitializeMainWindowAsync()
    {
        App.MainWindow.Title = dataService.GetFullName();
        App.MainWindow.Content = App.MainWindow.Content is null ? App.GetService<ShellPage>() : new Frame();
        App.MainWindow.MinHeight = settingsService.AppWindowMinHeight;
        App.MainWindow.MinWidth = settingsService.AppWindowMinWidth;
        var windowState = await settingsService.ReadAppWindowStateAsync();
        var windowPosition = await settingsService.ReadAppWindowPositionAsync();
        var windowHeight = await settingsService.ReadAppWindowHeightAsync();
        var windowWidth = await settingsService.ReadAppWindowWidthAsync();
        var displayArea = await displayService.GetDisplayAreaAsync();
        var hasCorrectPosition = windowPosition.X > 0 && windowPosition.Y > 0;
        var hasCorrectHeight = windowPosition.Y + windowHeight <= (displayArea?.WorkArea.Height ?? -1);
        var hasCorrectWidth = windowPosition.X + windowWidth <= (displayArea?.WorkArea.Width ?? -1);

        if (windowState == WindowState.Maximized)
        {
            App.MainWindow.WindowState = WindowState.Maximized;
        }
        else if (hasCorrectPosition && hasCorrectHeight && hasCorrectWidth)
        {
            App.MainWindow.MoveAndResize(x: windowPosition.X, y: windowPosition.Y, width: windowWidth, height: windowHeight);
        }
        else
        {
            App.MainWindow.Height = settingsService.AppWindowDefaultHeight;
            App.MainWindow.Width = settingsService.AppWindowDefaultWidth;
            App.MainWindow.CenterOnScreen();
        }

        await themesService.SetRequestedThemeAsync();
        TitleBarHelper.ApplySystemThemeToCaptionButtons();
        App.MainWindow.Activate();
        viewModel.NavigationService.NavigateTo(typeof(StartupViewModel).FullName!);
        await viewModel.ExecuteAsync();
    }
}
