// <copyright file="PageService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.ViewModels;
using SophiApp.Views;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> pages = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="PageService"/> class.
    /// </summary>
    public PageService()
    {
        Configure<StartupViewModel, StartupPage>();
        Configure<WmiStateViewModel, WmiStatePage>();
        Configure<Win11BuildLess22KViewModel, Win11BuildLess22KPage>();
        Configure<Win11Build22KViewModel, Win11Build22KPage>();
        Configure<Win11UbrLess2283ViewModel, Win11UbrLess2283Page>();
        Configure<PrivacyViewModel, PrivacyPage>();
        Configure<PersonalizationViewModel, PersonalizationPage>();
        Configure<SystemViewModel, SystemPage>();
        Configure<UwpViewModel, UwpPage>();
        Configure<TaskSchedulerViewModel, TaskSchedulerPage>();
        Configure<SecurityViewModel, SecurityPage>();
        Configure<ContextMenuViewModel, ContextMenuPage>();
        Configure<ProVersionViewModel, ProVersionPage>();
        Configure<SettingsViewModel, SettingsPage>();
    }

    /// <summary>
    /// Get page by type.
    /// </summary>
    /// <param name="key">Page type.</param>
    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (pages)
        {
            if (!pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (pages)
        {
            var key = typeof(VM).FullName!;
            if (pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {pages.First(p => p.Value == type).Key}");
            }

            pages.Add(key, type);
        }
    }
}
