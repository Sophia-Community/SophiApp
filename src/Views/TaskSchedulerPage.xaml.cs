// <copyright file="TaskSchedulerPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements the <see cref="TaskSchedulerPage"/> class.
/// </summary>
public sealed partial class TaskSchedulerPage : Page, INotifyPropertyChanged
{
    private double currentWidth = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskSchedulerPage"/> class.
    /// </summary>
    public TaskSchedulerPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        Models = new (ViewModel.JsonModels.FilterByTag(UICategoryTag.TaskScheduler));
    }

    /// <summary>
    /// Property change event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets view model for task scheduler page.
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

    /// <summary>
    /// Gets <see cref="UIModel"/> collection.
    /// </summary>
    public ObservableCollection<UIModel> Models { get; }

    private void PageTaskScheduler_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
    {
        CurrentWidth = ActualWidth;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
