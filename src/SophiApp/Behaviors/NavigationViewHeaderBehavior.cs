// <copyright file="NavigationViewHeaderBehavior.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;
using SophiApp.Contracts.Services;

/// <summary>
/// Implements the Navigation View header logic.
/// </summary>
public class NavigationViewHeaderBehavior : Behavior<NavigationView>
{
    /// <summary>
    /// <see cref="NavigationView.HeaderTemplate"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
    DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(NavigationViewHeaderBehavior), new PropertyMetadata(null, (d, e) => current!.UpdateHeaderTemplate()));

    /// <summary>
    /// <see cref="NavigationView.Header"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderContextProperty =
        DependencyProperty.RegisterAttached("HeaderContext", typeof(object), typeof(NavigationViewHeaderBehavior), new PropertyMetadata(null, (d, e) => current!.UpdateHeader()));

    /// <summary>
    /// <see cref="NavigationView.Header"/>.
    /// </summary>
    public static readonly DependencyProperty DefaultHeaderProperty =
        DependencyProperty.Register("DefaultHeader", typeof(object), typeof(NavigationViewHeaderBehavior), new PropertyMetadata(null, (d, e) => current!.UpdateHeader()));

    /// <summary>
    /// <see cref="NavigationView.Header"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderModeProperty =
        DependencyProperty.RegisterAttached("HeaderMode", typeof(bool), typeof(NavigationViewHeaderBehavior), new PropertyMetadata(NavigationViewHeaderMode.Always, (d, e) => current!.UpdateHeader()));

    private static NavigationViewHeaderBehavior? current;
    private Page? currentPage;

    /// <summary>
    /// Gets or sets default <see cref="NavigationView.Header"/> template.
    /// </summary>
    public DataTemplate? DefaultHeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets default <see cref="NavigationView.Header"/>.
    /// </summary>
    public object DefaultHeader
    {
        get => GetValue(DefaultHeaderProperty);
        set => SetValue(DefaultHeaderProperty, value);
    }

    /// <summary>
    /// Get <see cref="NavigationView.Header"/> mode.
    /// </summary>
    /// <param name="item">Represents content that a Frame control can navigate to.</param>
    public static NavigationViewHeaderMode GetHeaderMode(Page item) => (NavigationViewHeaderMode)item.GetValue(HeaderModeProperty);

    /// <summary>
    /// Set <see cref="NavigationView.Header"/> mode.
    /// </summary>
    /// <param name="item">Represents content that a Frame control can navigate to.</param>
    /// <param name="value">A <see cref="NavigationView.Header"/> mode.</param>
    public static void SetHeaderMode(Page item, NavigationViewHeaderMode value) => item.SetValue(HeaderModeProperty, value);

    /// <summary>
    /// Get <see cref="NavigationView.Header"/> context.
    /// </summary>
    /// <param name="item">Represents content that a Frame control can navigate to.</param>
    public static object GetHeaderContext(Page item) => item.GetValue(HeaderContextProperty);

    /// <summary>
    /// Set <see cref="NavigationView.Header"/> context.
    /// </summary>
    /// <param name="item">Represents content that a Frame control can navigate to.</param>
    /// <param name="value">A <see cref="NavigationView.Header"/> context.</param>
    public static void SetHeaderContext(Page item, object value) => item.SetValue(HeaderContextProperty, value);

    /// <summary>
    /// Get <see cref="NavigationView.Header"/> template.
    /// </summary>
    /// <param name="item">Represents content that a Frame control can navigate to.</param>
    public static DataTemplate GetHeaderTemplate(Page item) => (DataTemplate)item.GetValue(HeaderTemplateProperty);

    /// <summary>
    /// Set <see cref="NavigationView.Header"/> template.
    /// </summary>
    /// <param name="item">Represents content that a Frame control can navigate to.</param>
    /// <param name="value">A <see cref="NavigationView.Header"/> template.</param>
    public static void SetHeaderTemplate(Page item, DataTemplate value) => item.SetValue(HeaderTemplateProperty, value);

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        #pragma warning disable S2696 // Instance members should not write to "static" fields

        base.OnAttached();
        var navigationService = App.GetService<INavigationService>();
        navigationService.Navigated += OnNavigated;
        current = this;

        #pragma warning restore S2696 // Instance members should not write to "static" fields
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        var navigationService = App.GetService<INavigationService>();
        navigationService.Navigated -= OnNavigated;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame && frame.Content is Page page)
        {
            currentPage = page;
            UpdateHeader();
            UpdateHeaderTemplate();
        }
    }

    private void UpdateHeader()
    {
        if (currentPage != null)
        {
            var headerMode = GetHeaderMode(currentPage);
            if (headerMode == NavigationViewHeaderMode.Never)
            {
                AssociatedObject.Header = null;
                AssociatedObject.AlwaysShowHeader = false;
            }
            else
            {
                var headerFromPage = GetHeaderContext(currentPage);
                if (headerFromPage != null)
                {
                    AssociatedObject.Header = headerFromPage;
                }
                else
                {
                    AssociatedObject.Header = DefaultHeader;
                }

                if (headerMode == NavigationViewHeaderMode.Always)
                {
                    AssociatedObject.AlwaysShowHeader = true;
                }
                else
                {
                    AssociatedObject.AlwaysShowHeader = false;
                }
            }
        }
    }

    private void UpdateHeaderTemplate()
    {
        if (currentPage != null)
        {
            var headerTemplate = GetHeaderTemplate(currentPage);
            AssociatedObject.HeaderTemplate = headerTemplate ?? DefaultHeaderTemplate;
        }
    }
}
