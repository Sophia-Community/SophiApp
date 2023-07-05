// <copyright file="App.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using SophiApp.Activation;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.Notifications;
using SophiApp.Services;
using SophiApp.ViewModels;
using SophiApp.Views;

/// <summary>
/// <inheritdoc/>
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers
                services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                // Services
                services.AddSingleton<IAppNotificationService, AppNotificationService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IFileService, FileService>();
                services.AddSingleton<IAppContextService, AppContextService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();

                // Views and ViewModels
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<ProVersionViewModel>();
                services.AddTransient<ProVersionPage>();
                services.AddTransient<ContextMenuViewModel>();
                services.AddTransient<ContextMenuPage>();
                services.AddTransient<SecurityViewModel>();
                services.AddTransient<SecurityPage>();
                services.AddTransient<TaskSchedulerViewModel>();
                services.AddTransient<TaskSchedulerPage>();
                services.AddTransient<UwpViewModel>();
                services.AddTransient<UwpPage>();
                services.AddTransient<SystemViewModel>();
                services.AddTransient<SystemPage>();
                services.AddTransient<PersonalizationViewModel>();
                services.AddTransient<PersonalizationPage>();
                services.AddTransient<PrivacyViewModel>();
                services.AddTransient<PrivacyPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();

                // Configuration
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        GetService<IAppNotificationService>().Initialize();
        UnhandledException += App_UnhandledException;
    }

    /// <summary>
    /// Gets app main window.
    /// </summary>
    public static WindowEx MainWindow { get; } = new MainWindow();

    /// <summary>
    /// Gets or sets app titlebar.
    /// </summary>
    public static UIElement? AppTitlebar { get; set; }

    /// <summary>
    /// Gets <see cref="IHost"/>.
    /// </summary>
    public IHost Host
    {
        get;
    }

    /// <summary>
    /// Gets app service.
    /// </summary>
    /// <typeparam name="T">Service type.</typeparam>
    public static T GetService<T>()
        where T : class
    {
        if ((Current as App) !.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="args"><inheritdoc/>.</param>
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));
        await GetService<IActivationService>().ActivateAsync(args);
        MainWindow.Title = Host.Services.GetService<IAppContextService>()?.GetFullName() ?? "AppDisplayName".GetLocalized();
        MainWindow.CenterOnScreen();
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }
}
