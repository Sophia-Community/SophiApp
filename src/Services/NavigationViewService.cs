// <copyright file="NavigationViewService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;

using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.Views;

/// <inheritdoc/>
public class NavigationViewService : INavigationViewService
{
    private readonly INavigationService navigationService;
    private NavigationView? navigationView;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewService"/> class.
    /// </summary>
    /// <param name="navigationService"><inheritdoc/></param>
    public NavigationViewService(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    /// <summary>
    /// Gets collection of menu items displayed in the NavigationMenu.
    /// </summary>
    public IList<object>? MenuItems => navigationView?.MenuItems;

    /// <summary>
    /// Gets item that represents the entry point to app settings.
    /// </summary>
    public object? SettingsItem => navigationView?.SettingsItem;

    /// <summary>
    /// Initialize <see cref="NavigationView"/>.
    /// </summary>
    /// <param name="navigationView">Represents a container that enables navigation of app content.</param>
    [MemberNotNull(nameof(NavigationViewService.navigationView))]
    public void Initialize(NavigationView navigationView)
    {
        this.navigationView = navigationView;
        this.navigationView.BackRequested += OnBackRequested;
        this.navigationView.ItemInvoked += OnItemInvoked;
    }

    /// <summary>
    /// Gets selected <see cref="NavigationViewItem"/> in <see cref="NavigationView"/>.
    /// </summary>
    /// <param name="pageType">Item page type.</param>
    public NavigationViewItem? GetSelectedItem(Type pageType)
    {
        if (navigationView != null)
        {
            return GetSelectedItem(navigationView.MenuItems, pageType) ?? GetSelectedItem(navigationView.FooterMenuItems, pageType);
        }

        return null;
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => navigationService.GoBack();

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        // Assistive tools can invoke an item while the menu is blocked, so it is checked here too
        if (args.InvokedItemContainer is NavigationViewItem { IsHitTestVisible: false })
        {
            return;
        }

        if (args.IsSettingsInvoked)
        {
            navigationService.NavigateTo<SettingsPage>();
        }
        else if (args.InvokedItemContainer is NavigationViewItem item && NavigationHelper.GetNavigateTo(item) is Type pageType)
        {
            navigationService.NavigateTo(pageType);
        }
    }

    private NavigationViewItem? GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<NavigationViewItem>())
        {
            if (NavigationHelper.GetNavigateTo(item) == pageType)
            {
                return item;
            }

            var selectedChild = GetSelectedItem(item.MenuItems, pageType);
            if (selectedChild != null)
            {
                return selectedChild;
            }
        }

        return null;
    }
}
