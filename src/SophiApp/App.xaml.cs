// <copyright file="App.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using SophiApp.Activation;
using SophiApp.Contracts.Services;
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
                _ = services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers
                _ = services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                // Services
                _ = services.AddSingleton<IAppNotificationService, AppNotificationService>();
                _ = services.AddSingleton<ISettingsService, SettingsService>();
                _ = services.AddSingleton<IThemesService, ThemesService>();
                _ = services.AddSingleton<IInitializeService, InitializeService>();
                _ = services.AddSingleton<IPageService, PageService>();
                _ = services.AddSingleton<INavigationService, NavigationService>();
                _ = services.AddSingleton<IFileService, FileService>();
                _ = services.AddSingleton<ICommonDataService, CommonDataService>();
                _ = services.AddSingleton<IModelBuilderService, ModelBuilderService>();
                _ = services.AddTransient<INavigationViewService, NavigationViewService>();
                _ = services.AddTransient<IUriService, UriService>();
                _ = services.AddTransient<INetService, NetService>();
                _ = services.AddTransient<IInstrumentationService, InstrumentationService>();
                _ = services.AddTransient<IRequirementsService, RequirementsService>();
                _ = services.AddTransient<IUpdateService, UpdateService>();

                // Views and ViewModels
                _ = services.AddScoped<StartupViewModel>();
                _ = services.AddTransient<StartupPage>();
                _ = services.AddTransient<SettingsViewModel>();
                _ = services.AddTransient<SettingsPage>();
                _ = services.AddTransient<ProVersionViewModel>();
                _ = services.AddTransient<ProVersionPage>();
                _ = services.AddTransient<ContextMenuViewModel>();
                _ = services.AddTransient<ContextMenuPage>();
                _ = services.AddTransient<SecurityViewModel>();
                _ = services.AddTransient<SecurityPage>();
                _ = services.AddTransient<TaskSchedulerViewModel>();
                _ = services.AddTransient<TaskSchedulerPage>();
                _ = services.AddTransient<UwpViewModel>();
                _ = services.AddTransient<UwpPage>();
                _ = services.AddTransient<SystemViewModel>();
                _ = services.AddTransient<SystemPage>();
                _ = services.AddTransient<PersonalizationViewModel>();
                _ = services.AddTransient<PersonalizationPage>();
                _ = services.AddTransient<PrivacyViewModel>();
                _ = services.AddTransient<PrivacyPage>();
                _ = services.AddScoped<ShellViewModel>();
                _ = services.AddTransient<ShellPage>();
                _ = services.AddScoped<RequirementsFailureViewModel>();
                _ = services.AddTransient<RequirementsFailurePage>();

                // Configuration
                _ = services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        GetService<IAppNotificationService>()
            .Initialize();

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
    public IHost Host { get; init; }

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
    /// Allows only one copy of the app to run.
    /// </summary>
    public static void SetSingleInstance()
    {
        var appSingletonKey = "2e340960-5e58-4e2d-b0c1-0a1b54145345";
        var keyInstance = AppInstance.FindOrRegisterForKey(appSingletonKey);

        if (!keyInstance.IsCurrent)
        {
            Current.Exit();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="args"><inheritdoc/>.</param>
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await GetService<IInitializeService>().InitializeAsync(args);
        await GetService<ShellViewModel>().ExecuteAsync();
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }
}
