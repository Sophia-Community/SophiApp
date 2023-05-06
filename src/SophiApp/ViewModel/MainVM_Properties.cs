// <copyright file="MainVM_Properties.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModel
{
    using System;
    using System.Reflection;
    using System.Windows;
    using CommunityToolkit.Mvvm.Input;
    using SophiApp.Extensions;

    /// <summary>
    /// View model for a <see cref="MainWindow"/>.
    /// </summary>
    public partial class MainVM
    {
        private const string Edition = "Community | Private alpha";
        private readonly string name = Assembly.GetExecutingAssembly().GetName().Name!;
        private readonly Version version = Assembly.GetExecutingAssembly().GetName().Version!;

        /// <summary>
        /// Gets app name and version.
        /// </summary>
        public string FullName => $"{name} {version.ToShortString()} | {Edition}";

        /// <summary>
        /// Gets close window command.
        /// </summary>
        public RelayCommand CloseWindowCommand => new (CloseWindow);

        private void CloseWindow() => Application.Current.MainWindow.Close();
    }
}
