// <copyright file="App.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
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
        // The language is chosen before the first text is loaded
        AppLanguage.ApplySaved();
        InitializeComponent();
        var services = new ServiceCollection();

        // Services
        _ = services.AddSingleton<ICommonDataService, CommonDataService>();
        _ = services.AddSingleton<IFileService, FileService>();
        _ = services.AddSingleton<IFirewallService, FirewallService>();
        _ = services.AddSingleton<IInitializeService, InitializeService>();
        _ = services.AddSingleton<IInstrumentationService, InstrumentationService>();
        _ = services.AddSingleton<ILoggerService, LoggerService>();
        _ = services.AddSingleton<IModelService, ModelService>();
        _ = services.AddSingleton<INavigationService, NavigationService>();
        _ = services.AddSingleton<IRequirementsService, RequirementsService>();
        _ = services.AddSingleton<ISettingsService, SettingsService>();
        _ = services.AddSingleton<IStatusService, StatusService>();
        _ = services.AddSingleton<IThemesService, ThemesService>();

        // One for the whole app: it keeps the download that is started at startup
        _ = services.AddSingleton<IWslService, WslService>();
        _ = services.AddTransient<IAppNotificationService, AppNotificationService>();
        _ = services.AddTransient<IAppxPackagesService, AppxPackagesService>();
        _ = services.AddTransient<ICursorsService, CursorsService>();
        _ = services.AddTransient<IDisplayService, DisplayService>();
        _ = services.AddTransient<IGroupPolicyService, GroupPolicyService>();
        // One for the whole app: it knows the download in progress, which the user can cancel
        _ = services.AddSingleton<IHttpService, HttpService>();
        _ = services.AddTransient<INavigationViewService, NavigationViewService>();
        _ = services.AddTransient<IDnsService, DnsService>();
        _ = services.AddTransient<IAssociationService, AssociationService>();
        _ = services.AddTransient<INetworkAdapterService, NetworkAdapterService>();
        _ = services.AddTransient<IOneDriveService, OneDriveService>();
        _ = services.AddTransient<IOsService, OsService>();
        _ = services.AddTransient<IPowerShellService, PowerShellService>();
        _ = services.AddTransient<IProcessService, ProcessService>();
        _ = services.AddTransient<IRedistributablePackageService, RedistributablePackageService>();
        _ = services.AddTransient<IRegistryService, RegistryService>();
        _ = services.AddTransient<IScheduledTaskService, ScheduledTaskService>();
        _ = services.AddTransient<IUpdateService, UpdateService>();
        _ = services.AddTransient<IXmlService, XmlService>();

        // ViewModels
        _ = services.AddSingleton<AppActivity>();
        _ = services.AddSingleton<ChosenSettingsViewModel>();
        _ = services.AddSingleton<PageNotesViewModel>();
        _ = services.AddSingleton<SearchViewModel>();
        _ = services.AddSingleton<ShellViewModel>();
        _ = services.AddSingleton<StartupViewModel>();
        _ = services.AddTransient<FatalErrorViewModel>();
        _ = services.AddTransient<RequirementsFailureModel>();

        _ = services.AddTransient<SettingsViewModel>();

        // Views
        _ = services.AddTransient<ShellPage>();
        Services = services.BuildServiceProvider();

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
    /// Gets <see cref="IStatusService"/>.
    /// </summary>
    public static IStatusService Status { get; } = GetService<IStatusService>();

    /// <summary>
    /// Gets the services of the app.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets app service.
    /// </summary>
    /// <typeparam name="T">Service type.</typeparam>
    public static T GetService<T>()
        where T : class
    {
        if ((Current as App)!.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in App.xaml.cs.");
        }

        return service;
    }

    /// <inheritdoc/>
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        GetService<IAppNotificationService>().RegisterAsToastSender();
        var initializeService = GetService<IInitializeService>();
        await initializeService.InitializeServicesDataAsync(args);
        await initializeService.InitializeMainWindowAsync();
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.LogUnhandledException(e.Exception);
        GetService<ShellViewModel>().FatalErrorHandling();
    }
}
