// <copyright file="App.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SophiApp.ViewModel;
    using System;
    using System.Windows;

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
                _ = services.AddTransient<MainVM>();
            })
            .Build();

            var vm = Host.Services.GetService<MainVM>();
            MainWindow = new MainWindow(vm!);
        }

        /// <summary>
        /// Gets a <see cref="IHost"/>.
        /// </summary>
        public IHost Host { get; init; }

        /// <summary>
        /// Gets <see cref="MainWindow"/>.
        /// </summary>
        public new MainWindow MainWindow { get; init; }
    }
}