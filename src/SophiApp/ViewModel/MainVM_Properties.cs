// <copyright file="MainVM_Properties.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModel
{
    using System;
    using System.Reflection;
    using CommunityToolkit.Mvvm.ComponentModel;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// View model for a <see cref="MainWindow"/>.
    /// </summary>
    public partial class MainVM
    {
        private const string Edition = "Community | Private alpha";

        private readonly string name = Assembly.GetExecutingAssembly().GetName().Name!;
        private readonly Version version = Assembly.GetExecutingAssembly().GetName().Version!;
        [ObservableProperty]
        private PageTag activePage = PageTag.Privacy;

        /// <summary>
        /// Gets app name and version.
        /// </summary>
        public string FullName => $"{name} {version.ToShortString()} | {Edition}";
    }
}
