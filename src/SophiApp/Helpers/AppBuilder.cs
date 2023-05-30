// <copyright file="AppBuilder.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System;

    /// <summary>
    /// Entry point to start the application.
    /// </summary>
    public static class AppBuilder
    {
        /// <summary>
        /// Launches the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            _ = app.Run(app.MainWindow);
        }
    }
}
