// <copyright file="App.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using SophiApp.Contracts.Services;
using SophiApp.Models;
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
                // Services
                _ = services.AddScoped<IInstrumentationService, InstrumentationService>();
                _ = services.AddScoped<ILoggerService, LoggerService>();
                _ = services.AddSingleton<ICommonDataService, CommonDataService>();
                _ = services.AddSingleton<IFileService, FileService>();
                _ = services.AddSingleton<IFirewallService, FirewallService>();
                _ = services.AddSingleton<IInitializeService, InitializeService>();
                _ = services.AddSingleton<IModelService, ModelService>();
                _ = services.AddSingleton<INavigationService, NavigationService>();
                _ = services.AddSingleton<IPageService, PageService>();
                _ = services.AddSingleton<ISettingsService, SettingsService>();
                _ = services.AddSingleton<IThemesService, ThemesService>();
                _ = services.AddTransient<IAppNotificationService, AppNotificationService>();
                _ = services.AddTransient<IAppxPackagesService, AppxPackagesService>();
                _ = services.AddTransient<IDiskService, DiskService>();
                _ = services.AddTransient<IGroupPolicyService, GroupPolicyService>();
                _ = services.AddTransient<IHttpService, HttpService>();
                _ = services.AddTransient<INavigationViewService, NavigationViewService>();
                _ = services.AddTransient<IOsService, OsService>();
                _ = services.AddTransient<IPowerShellService, PowerShellService>();
                _ = services.AddTransient<IProcessService, ProcessService>();
                _ = services.AddTransient<IRegistryService, RegistryService>();
                _ = services.AddTransient<IRequirementsService, RequirementsService>();
                _ = services.AddTransient<IScheduledTaskService, ScheduledTaskService>();
                _ = services.AddTransient<IUpdateService, UpdateService>();
                _ = services.AddTransient<IUriService, UriService>();
                _ = services.AddTransient<IXmlService, XmlService>();

                // ViewModels
                _ = services.AddScoped<RequirementsFailureViewModel>();
                _ = services.AddScoped<ShellViewModel>();
                _ = services.AddScoped<StartupViewModel>();
                _ = services.AddTransient<ContextMenuViewModel>();
                _ = services.AddTransient<PersonalizationViewModel>();
                _ = services.AddTransient<PrivacyViewModel>();
                _ = services.AddTransient<ProVersionViewModel>();
                _ = services.AddTransient<SearchViewModel>();
                _ = services.AddTransient<SecurityViewModel>();
                _ = services.AddTransient<SettingsViewModel>();
                _ = services.AddTransient<SystemViewModel>();
                _ = services.AddTransient<TaskSchedulerViewModel>();
                _ = services.AddTransient<UwpViewModel>();

                // Views
                _ = services.AddTransient<ContextMenuPage>();
                _ = services.AddTransient<PersonalizationPage>();
                _ = services.AddTransient<PrivacyPage>();
                _ = services.AddTransient<ProVersionPage>();
                _ = services.AddTransient<RequirementsFailurePage>();
                _ = services.AddTransient<SearchPage>();
                _ = services.AddTransient<SecurityPage>();
                _ = services.AddTransient<SettingsPage>();
                _ = services.AddTransient<ShellPage>();
                _ = services.AddTransient<StartupPage>();
                _ = services.AddTransient<SystemPage>();
                _ = services.AddTransient<TaskSchedulerPage>();
                _ = services.AddTransient<UwpPage>();

                // Configuration
                _ = services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        UnhandledException += App_UnhandledException;
    }

    /// <summary>
    /// Gets app main window.
    /// </summary>
    public static WindowEx MainWindow { get; } = new MainWindow();

    /// <summary>
    /// Gets or sets app title bar.
    /// </summary>
    public static UIElement? AppTitlebar { get; set; }

    /// <summary>
    /// Gets <see cref="ILoggerService"/>.
    /// </summary>
    public static ILoggerService Logger { get; } = GetService<ILoggerService>();

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

    /// <inheritdoc/>
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        GetService<IAppNotificationService>().RegisterAsToastSender("SophiApp");
        await GetService<IInitializeService>().InitializeAsync(args);
        await GetService<ShellViewModel>().ExecuteAsync();
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.LogUnhandledException(e.Exception);
    }
}
