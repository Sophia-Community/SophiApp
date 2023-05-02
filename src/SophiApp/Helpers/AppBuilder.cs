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
        /// <returns>The <see cref="int"/> application exit code that is returned to the operating system
        /// when the application shuts down. By default, the exit code value is 0.</returns>
        [STAThread]
        public static int Main()
        {
            var app = new App();
            app.InitializeComponent();
            return app.Run(app.MainWindow);
        }
    }
}