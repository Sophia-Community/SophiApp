// <copyright file="InitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Views;

/// <inheritdoc/>
public class InitializeService : IInitializeService
{
    private readonly IThemesService themeSelectorService;
    private readonly ICommonDataService commonDataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeService"/> class.
    /// </summary>
    /// <param name="themeSelectorService">A service for working with app themes.</param>
    /// <param name="commonDataService">A service for working with common app data.</param>
    public InitializeService(
        IThemesService themeSelectorService,
        ICommonDataService commonDataService)
    {
        this.themeSelectorService = themeSelectorService;
        this.commonDataService = commonDataService;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="args"><inheritdoc/></param>
    public async Task InitializeAsync(object args)
    {
        InitializeMainWindow();
        await InitializeThemeAsync();
    }

    private void InitializeMainWindow()
    {
        App.MainWindow.Title = commonDataService.GetFullName();

        if (App.MainWindow.Content == null)
        {
            var shell = App.GetService<ShellPage>() as UIElement;
            App.MainWindow.Content = shell ?? new Frame();
        }

        App.MainWindow.CenterOnScreen();
        App.MainWindow.Activate();
    }

    private async Task InitializeThemeAsync()
    {
        await themeSelectorService.InitializeAsync();
        await themeSelectorService.SetRequestedThemeAsync();
    }
}
