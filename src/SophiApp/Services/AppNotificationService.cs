// <copyright file="AppNotificationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Notifications;

using Microsoft.Win32;
using SophiApp.Contracts.Services;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

/// <inheritdoc/>
public class AppNotificationService : IAppNotificationService
{
    /// <inheritdoc/>
    public void Register()
    {
        try
        {
            Registry.ClassesRoot.CreateSubKey(subkey: "AppUserModelId\\SophiApp", writable: true).SetValue("DisplayName", "SophiApp", RegistryValueKind.String);
            Registry.ClassesRoot.OpenSubKey(name: "AppUserModelId\\SophiApp", writable: true)?.SetValue("ShowInSettings", 0, RegistryValueKind.DWord);
        }
        catch (Exception)
        {
            // TODO: Log exception here.
            throw;
        }
    }

    /// <inheritdoc/>
    public void Show(string payload)
    {
        var xml = new XmlDocument();
        xml.LoadXml(payload);
        var toast = new ToastNotification(xml);
        ToastNotificationManager.CreateToastNotifier("SophiApp").Show(toast);
    }
}
