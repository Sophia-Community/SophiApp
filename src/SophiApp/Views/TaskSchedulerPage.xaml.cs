// <copyright file="TaskSchedulerPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="TaskSchedulerPage"/> class.
/// </summary>
public sealed partial class TaskSchedulerPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskSchedulerPage"/> class.
    /// </summary>
    public TaskSchedulerPage()
    {
        ViewModel = App.GetService<TaskSchedulerViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="TaskSchedulerViewModel"/>.
    /// </summary>
    public TaskSchedulerViewModel ViewModel
    {
        get;
    }
}
