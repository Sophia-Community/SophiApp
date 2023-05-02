// <copyright file="MainVM_Properties.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModel
{
    using SophiApp.Extensions;
    using System;
    using System.Reflection;

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
    }
}