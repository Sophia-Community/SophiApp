// <copyright file="UwpPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements the <see cref="UwpPage"/> class.
/// </summary>
public sealed partial class UwpPage : Page, INotifyPropertyChanged
{
    private double currentWidth = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="UwpPage"/> class.
    /// </summary>
    public UwpPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
    }

    /// <summary>
    /// Property change event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets view model for UWP page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets a value indicating current width.
    /// </summary>
    public double CurrentWidth
    {
        get => currentWidth;
        private set
        {
            currentWidth = value;
            OnPropertyChanged(nameof(CurrentWidth));
        }
    }

    private void PageUwp_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
    {
        CurrentWidth = ActualWidth;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
