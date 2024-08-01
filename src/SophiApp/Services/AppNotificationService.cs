// <copyright file="AppNotificationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Notifications;

using Microsoft.Win32;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

/// <inheritdoc/>
public class AppNotificationService : IAppNotificationService
{
    /// <inheritdoc/>
    public void EnableToastNotification()
    {
        var explorerPoliciesPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
        var scriptHostPath = "Software\\Microsoft\\Windows Script Host\\Settings";

        Registry.CurrentUser.OpenSubKey(explorerPoliciesPath, true)?.DeleteValue("DisableNotificationCenter", false);
        Registry.LocalMachine.OpenSubKey(explorerPoliciesPath, true)?.DeleteValue("DisableNotificationCenter", false);

        Registry.CurrentUser.OpenSubKey(scriptHostPath, true)?.DeleteValue("Enabled", false);
        Registry.LocalMachine.OpenSubKey(scriptHostPath, true)?.DeleteValue("Enabled", false);

        Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", true)?.DeleteValue("ToastEnabled", false);
    }

    /// <inheritdoc/>
    public void RegisterAsToastSender(string name)
    {
        try
        {
            var appId = $"AppUserModelId\\{name}";
            var actionCenterSetting = $"Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\{name}";
            Registry.CurrentUser.OpenOrCreateSubKey(actionCenterSetting).SetValue("ShowInActionCenter", 1, RegistryValueKind.DWord);
            Registry.ClassesRoot.OpenOrCreateSubKey(appId).SetValue("DisplayName", name, RegistryValueKind.String);
            Registry.ClassesRoot.OpenSubKey(appId, true)?.SetValue("ShowInSettings", 0, RegistryValueKind.DWord);
        }
        catch (Exception ex)
        {
            App.Logger.LogRegisterNotificationSenderException(ex);
        }
    }

    /// <inheritdoc/>
    public void RegisterCleanupProtocol()
    {
        var commandPath = "WindowsCleanup\\shell\\open\\command";
        var commandValue = "powershell.exe -Command \"& {Start-ScheduledTask -TaskPath ''\\Sophia\\'' -TaskName ''Windows Cleanup''}\"' -Force";
        Registry.ClassesRoot.OpenOrCreateSubKey(commandPath).SetValue(string.Empty, commandValue, RegistryValueKind.String);
        Registry.ClassesRoot.OpenSubKey("WindowsCleanup", true)?.SetValue(string.Empty, "URL:WindowsCleanup", RegistryValueKind.String);
        Registry.ClassesRoot.OpenSubKey("WindowsCleanup", true)?.SetValue("URL Protocol", string.Empty, RegistryValueKind.String);
        Registry.ClassesRoot.OpenSubKey("WindowsCleanup", true)?.SetValue("EditFlags", 2162688, RegistryValueKind.DWord);
    }

    /// <inheritdoc/>
    public void Show(string payload)
    {
        var xml = new XmlDocument();
        xml.LoadXml(payload);
        var toast = new ToastNotification(xml);
        ToastNotificationManager.CreateToastNotifier("SophiApp")
            .Show(toast);
    }
}
