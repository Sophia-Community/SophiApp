// <copyright file="NavigationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.Contracts.ViewModels;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Views;
using System.Diagnostics.CodeAnalysis;

/// <inheritdoc/>
public class NavigationService : INavigationService
{
    private object? lastParameterUsed;
    private Frame? frame;

    /// <summary>
    /// Occurs when the frame has navigated to a page.
    /// </summary>
    public event NavigatedEventHandler? Navigated;

    /// <inheritdoc/>
    public Frame? Frame
    {
        get
        {
            if (frame == null)
            {
                frame = App.MainWindow.Content as Frame;
                RegisterFrameEvents();
            }

            return frame;
        }

        set
        {
            UnregisterFrameEvents();
            frame = value;
            RegisterFrameEvents();
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Frame), nameof(frame))]
    public bool CanGoBack => Frame != null && Frame.CanGoBack;

    /// <inheritdoc/>
    public bool GoBack()
    {
        if (CanGoBack)
        {
            var vmBeforeNavigation = frame.GetPageViewModel();
            frame.GoBack();
            if (vmBeforeNavigation is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedFrom();
            }

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool NavigateTo<TPage>(object? parameter = null, bool clearHistory = false, bool disablePageAnimation = false)
        where TPage : Page
        => NavigateTo(typeof(TPage), parameter, clearHistory, disablePageAnimation);

    /// <inheritdoc/>
    public bool NavigateTo(Type pageType, object? parameter = null, bool clearHistory = false, bool disablePageAnimation = false)
    {
        if (frame != null && (disablePageAnimation || frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(lastParameterUsed))))
        {
            frame.Tag = clearHistory;
            var vmBeforeNavigation = frame.GetPageViewModel();
            // With the animations of Windows turned off the pages change at once
            var navigateAnimation = disablePageAnimation || !SystemAnimations.AreEnabled ? new SuppressNavigationTransitionInfo() : null;
            var navigated = frame.Navigate(pageType, parameter, navigateAnimation);
            if (navigated)
            {
                lastParameterUsed = parameter;
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        return false;
    }

    private void RegisterFrameEvents()
    {
        if (frame != null)
        {
            frame.Navigated += OnNavigated;
        }
    }

    private void UnregisterFrameEvents()
    {
        if (frame != null)
        {
            frame.Navigated -= OnNavigated;
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame page)
        {
            var clearNavigation = (bool)page.Tag;

            if (clearNavigation)
            {
                page.BackStack.Clear();
            }

            if (FrameNavigation.GetPageViewModel(page) is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.Parameter);
            }

            if (e.SourcePageType != typeof(StartupPage))
            {
                App.Logger.LogNavigateToPage(e.SourcePageType.Name);
            }

            Navigated?.Invoke(sender, e);
        }
    }
}
