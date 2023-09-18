// <copyright file="InitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Activation;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.Views;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class InitializeService : IInitializeService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> defaultHandler;
    private readonly IEnumerable<IActivationHandler> activationHandlers;
    private readonly IThemeSelectorService themeSelectorService;
    private readonly ICommonDataService commonDataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeService"/> class.
    /// </summary>
    /// <param name="defaultHandler">Default activation handler.</param>
    /// <param name="activationHandlers">Another activation handlers.</param>
    /// <param name="themeSelectorService">A service for working with app themes.</param>
    /// <param name="commonDataService">A service for working with common app data.</param>
    /// <param name="modelBuilderService">MVVM pattern model builder service.</param>
    public InitializeService(
        ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers,
        IThemeSelectorService themeSelectorService,
        ICommonDataService commonDataService)
    {
        this.defaultHandler = defaultHandler;
        this.activationHandlers = activationHandlers;
        this.themeSelectorService = themeSelectorService;
        this.commonDataService = commonDataService;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="args"><inheritdoc/></param>
    public async Task InitializeAsync(object args)
    {
        await InitializeAsync();
        InitializeMainWindow();
        await InitializeHandleAsync(args);
        await StartupAsync();
    }

    private async Task InitializeAsync()
    {
        await themeSelectorService.InitializeAsync()
            .ConfigureAwait(false);
        await Task.CompletedTask;
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

    private async Task InitializeHandleAsync(object activationArgs)
    {
        var activationHandler = activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (defaultHandler.CanHandle(activationArgs))
        {
            await defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task StartupAsync()
    {
        await themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
