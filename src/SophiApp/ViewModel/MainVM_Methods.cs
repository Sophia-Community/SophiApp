// <copyright file="MainVM_Methods.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModel
{
    using System;
    using System.Windows;
    using CommunityToolkit.Mvvm.Input;
    using SophiApp.Helpers;
    using SophiApp.UI;

    /// <summary>
    /// View model for a <see cref="MainWindow"/>.
    /// </summary>
    public partial class MainVM
    {
        /// <summary>
        /// Appears when the <see cref="CategoryButton"/> is clicked.
        /// </summary>
        [RelayCommand]
        private void CategoryButtonClicked(string tag) => ActivePage = (PageTag)Enum.Parse(typeof(PageTag), tag);

        /// <summary>
        /// Appears when the <see cref="MainWindow"/> is closed.
        /// </summary>
        [RelayCommand]
        private void CloseWindow() => Application.Current.MainWindow.Close();
    }
}
