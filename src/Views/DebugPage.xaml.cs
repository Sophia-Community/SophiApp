// <copyright file="DebugPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="DebugPage"/> class.
    /// </summary>
    public sealed partial class DebugPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugPage"/> class.
        /// </summary>
        public DebugPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<DebugViewModel>();
        }

        /// <summary>
        /// Gets <see cref="DebugViewModel"/>.
        /// </summary>
        public DebugViewModel ViewModel { get; }

        private void RequirementActionsRadioButton_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DebugRequirementActionsTip.IsOpen = true;
            var button = sender as RadioButton;
            ViewModel.SaveDebugRequirementActionCommand.Execute(button!.Content as string);
        }
    }
}
