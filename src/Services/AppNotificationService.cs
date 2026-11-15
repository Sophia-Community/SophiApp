// <copyright file="AppNotificationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;

using Microsoft.Win32;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;

/// <inheritdoc/>
public class AppNotificationService : IAppNotificationService
{
    private readonly IGroupPolicyService groupPolicyService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppNotificationService"/> class.
    /// </summary>
    /// <param name="groupPolicyService">A service for working with group policy API.</param>
    public AppNotificationService(IGroupPolicyService groupPolicyService) => this.groupPolicyService = groupPolicyService;

    /// <inheritdoc/>
    public void EnableToastNotification()
    {
        // Enable notifications
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", "ToastEnabled");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\Windows.ActionCenter.SmartOptOut", "Enable");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\SophiApp", "ShowBanner");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\SophiApp", "ShowInActionCenter");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\SophiApp", "Enabled");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\Sophia", "ShowBanner");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\Sophia", "ShowInActionCenter");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\Sophia", "Enabled");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\SystemSettings\\AccountNotifications", "EnableAccountNotifications");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableNotificationCenter");
        Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableNotificationCenter");
        Registry.CurrentUser.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", "NoToastApplicationNotification");
        // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
        groupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableNotificationCenter", LGPOScope.Computer, LGPOScope.User);
    }

    /// <inheritdoc/>
    public void RegisterAsToastSender()
    {
        try
        {
            // Register app. ShowInSettings determines whether the app can be seen in Settings where the user can turn notifications on or off
            Registry.ClassesRoot.CreateKeyAndSetValue("AppUserModelId\\SophiApp", "DisplayName", "SophiApp for Windows", RegistryValueKind.String);
            Registry.ClassesRoot.CreateKeyAndSetValue("AppUserModelId\\Sophia", "DisplayName", "Sophia", RegistryValueKind.String);
            Registry.ClassesRoot.SetValueIfKeyExists("AppUserModelId\\SophiApp", "ShowInSettings", 0, RegistryValueKind.DWord);
            Registry.ClassesRoot.SetValueIfKeyExists("AppUserModelId\\Sophia", "ShowInSettings", 0, RegistryValueKind.DWord);
        }
        catch (Exception ex)
        {
            App.Logger.LogRegisterNotificationSenderException(ex);
        }
    }

    /// <inheritdoc/>
    public void RegisterWindowsCleanupAsToastSender()
    {
        // Start the "Windows Cleanup" task if the "Run" button clicked
        Registry.ClassesRoot.CreateKeyAndSetValue("WindowsCleanup\\shell\\open\\command", string.Empty, @"powershell.exe -Command ""& {Start-ScheduledTask -TaskPath '\Sophia\' -TaskName 'Windows Cleanup'}""", RegistryValueKind.String);
        // Register the "WindowsCleanup" protocol to be able to run the scheduled task by clicking the "Run" button in a toast
        Registry.ClassesRoot.SetValueIfKeyExists("WindowsCleanup", string.Empty, "URL:WindowsCleanup", RegistryValueKind.String);
        Registry.ClassesRoot.SetValueIfKeyExists("WindowsCleanup", "URL Protocol", string.Empty, RegistryValueKind.String);
        Registry.ClassesRoot.SetValueIfKeyExists("WindowsCleanup", "EditFlags", 2162688, RegistryValueKind.DWord);
    }

    /// <inheritdoc/>
    public void UnregisterCleanupProtocol() => Registry.ClassesRoot.DeleteSubKeyTree("WindowsCleanup", false);
}
