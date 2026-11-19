// <copyright file="AnimatedLogo.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using System.Diagnostics;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// The animated app logo shown while the app starts, when it cannot continue and when the search finds nothing.
    /// </summary>
    public sealed partial class AnimatedLogo : UserControl
    {
        // One clock for all pages: the logo of the next page goes on from the frame where the previous one stopped
        private static readonly Stopwatch Clock = Stopwatch.StartNew();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedLogo"/> class.
        /// </summary>
        public AnimatedLogo()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            var duration = Player.Duration.Ticks;

            if (duration > 0)
            {
                // Play the rest of the current pass, then loop the whole animation
                await Player.PlayAsync((Clock.Elapsed.Ticks % duration) / (double)duration, 1, false);
            }

            if (IsLoaded)
            {
                _ = Player.PlayAsync(0, 1, true);
            }
        }
    }
}
