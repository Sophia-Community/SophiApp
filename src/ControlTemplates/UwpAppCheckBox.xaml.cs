// <copyright file="UwpAppCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A tile with the logo and the names of a UWP app and a check box.
    /// </summary>
    public sealed partial class UwpAppCheckBox : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="IsAppEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAppEnabledProperty = DependencyProperty.Register(
            nameof(IsAppEnabled),
            typeof(bool),
            typeof(UwpAppCheckBox),
            new PropertyMetadata(true, (sender, args) => ((UwpAppCheckBox)sender).ShowAvailability((bool)args.NewValue)));

        /// <summary>
        /// Initializes a new instance of the <see cref="UwpAppCheckBox"/> class.
        /// </summary>
        public UwpAppCheckBox()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();

            // Follows the model: an uninstalled app becomes unavailable
            SetBinding(IsAppEnabledProperty, new Binding { Path = new PropertyPath("IsEnabled") });

            // While the app is busy the mouse is blocked by the page, and the keyboard is blocked here.
            // The control is not disabled for that: a check box that was disabled and enabled again keeps a stale check mark
            PreviewKeyDown += (_, args) => args.Handled = !ViewModel.NavigationViewHitTestVisible;
            PreviewKeyUp += (_, args) => args.Handled = !ViewModel.NavigationViewHitTestVisible;
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the app can still be chosen.
        /// </summary>
        public bool IsAppEnabled
        {
            get => (bool)GetValue(IsAppEnabledProperty);
            set => SetValue(IsAppEnabledProperty, value);
        }

        private void ShowAvailability(bool isEnabled)
        {
            // The logo and the names are not controls, so they are dimmed by hand
            AppLogo.Opacity = isEnabled ? 1 : 0.4;
            AppNames.Opacity = isEnabled ? 1 : 0.4;
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Name);

        // The name the package is uninstalled by, such as "Microsoft.Paint"
        private void CopyPackageName_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);
    }
}
