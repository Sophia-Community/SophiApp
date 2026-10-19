// <copyright file="NavigationViewService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.ViewModels;

/// <inheritdoc/>
public class NavigationViewService : INavigationViewService
{
    private readonly INavigationService navigationService;
    private readonly IPageService pageService;
    private NavigationView? navigationView;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewService"/> class.
    /// </summary>
    /// <param name="navigationService"><inheritdoc/></param>
    /// <param name="pageService"><inheritdoc/></param>
    public NavigationViewService(INavigationService navigationService, IPageService pageService)
    {
        this.navigationService = navigationService;
        this.pageService = pageService;
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
    /// Unregister menu item events.
    /// </summary>
    public void UnregisterEvents()
    {
        if (navigationView != null)
        {
            navigationView.BackRequested -= OnBackRequested;
            navigationView.ItemInvoked -= OnItemInvoked;
        }
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
        if (args.IsSettingsInvoked)
        {
            navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
        }
        else
        {
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;

            if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
            {
                navigationService.NavigateTo(pageKey);
            }
        }
    }

    private NavigationViewItem? GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<NavigationViewItem>())
        {
            if (IsMenuItemForPageType(item, pageType))
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

    private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
    {
        if (menuItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
        {
            return pageService.GetPageType(pageKey) == sourcePageType;
        }

        return false;
    }
}
