// <copyright file="InitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Views;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class InitializeService : IInitializeService
{
    private readonly IThemesService themeSelectorService;
    private readonly ICommonDataService commonDataService;
    private readonly IModelBuilderService modelBuilderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeService"/> class.
    /// </summary>
    /// <param name="themeSelectorService">A service for working with app themes.</param>
    /// <param name="commonDataService">A service for working with common app data.</param>
    /// <param name="modelBuilderService">MVVM pattern model builder service.</param>
    public InitializeService(
        IThemesService themeSelectorService,
        ICommonDataService commonDataService,
        IModelBuilderService modelBuilderService)
    {
        this.themeSelectorService = themeSelectorService;
        this.commonDataService = commonDataService;
        this.modelBuilderService = modelBuilderService;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="args"><inheritdoc/></param>
    public async Task InitializeAsync(object args)
    {
        InitializeMainWindow();
        await InitializeThemeAsync();
        await BuildModelsAsync();
    }

    private void InitializeMainWindow()
    {
        var title = $"{commonDataService?.GetFullName()} {commonDataService?.GetDelimiter()} {commonDataService?.GetVersionName()}" ?? "AppDisplayName".GetLocalized();
        App.MainWindow.Title = title;

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

    private async Task BuildModelsAsync()
    {
        await modelBuilderService.BuildModelsAsync();
    }
}
