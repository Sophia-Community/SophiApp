// <copyright file="SearchPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Extensions;
    using SophiApp.ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        public SearchPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ShellViewModel>();
        }

        /// <summary>
        /// Gets view model for search page.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        /// <summary>
        /// Correct the vertical offset so that the last <see cref="FrameworkElement"/> in the sequence fits on the UI.
        /// </summary>
        public void CorrectScrollViewPosition()
        {
            var modelMaxViewId = ViewModel.FoundModels.Max(model => model.ViewId);

            if (ViewModel.ApplicableModels[0].ViewId == modelMaxViewId)
            {
                this.FindName<ScrollView>("SearchScrollView")?.VerticalOffsetCorrection();
            }
        }
    }
}
