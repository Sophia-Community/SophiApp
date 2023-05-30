// <copyright file="App.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp
{
    using System;
    using System.Windows;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SophiApp.ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
            : base()
        {
            Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
            {
                // Views and Models
                _ = services.AddSingleton<MainVM>();
            })
            .Build();

            MainWindow = new MainWindow(Host.Services.GetRequiredService<MainVM>());
        }

        /// <summary>
        /// Gets a <see cref="IHost"/>.
        /// </summary>
        public static IHost? Host { get; private set; }

        /// <summary>
        /// Gets <see cref="MainWindow"/>.
        /// </summary>
        public new MainWindow MainWindow { get; init; }
    }
}
