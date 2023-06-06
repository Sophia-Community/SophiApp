// <copyright file="AppNotificationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Notifications;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Windows.AppNotifications;
using SophiApp.Contracts.Services;

public class AppNotificationService : IAppNotificationService
{
    private readonly INavigationService navigationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppNotificationService"/> class.
    /// </summary>
    /// <param name="navigationService"><inheritdoc/></param>
    public AppNotificationService(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="AppNotificationService"/> class.
    /// </summary>
    ~AppNotificationService()
    {
        Unregister();
    }

    public void Initialize()
    {
        AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

        AppNotificationManager.Default.Register();
    }

    public void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        // TODO: Handle notification invocations when your app is already running.

        //// // Navigate to a specific page based on the notification arguments.
        //// if (ParseArguments(args.Argument)["action"] == "Settings")
        //// {
        ////    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        ////    {
        ////        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
        ////    });
        //// }

        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            App.MainWindow.ShowMessageDialogAsync("TODO: Handle notification invocations when your app is already running.", "Notification Invoked");

            App.MainWindow.BringToFront();
        });
    }

    public bool Show(string payload)
    {
        var appNotification = new AppNotification(payload);

        AppNotificationManager.Default.Show(appNotification);

        return appNotification.Id != 0;
    }

    public NameValueCollection ParseArguments(string arguments)
    {
        return HttpUtility.ParseQueryString(arguments);
    }

    public void Unregister()
    {
        AppNotificationManager.Default.Unregister();
    }
}
