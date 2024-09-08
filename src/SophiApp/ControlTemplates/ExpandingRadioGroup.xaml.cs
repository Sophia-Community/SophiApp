// <copyright file="ExpandingRadioGroup.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="ExpandingRadioGroup"/> element.
    /// </summary>
    public sealed partial class ExpandingRadioGroup : UserControl
    {
        // TODO: Check DataTemplate binding in WinUI new release: https://github.com/microsoft/microsoft-ui-xaml/issues/560

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandingRadioGroup"/> class.
        /// </summary>
        public ExpandingRadioGroup()
        {
            this.InitializeComponent();
            FontOptions = App.GetService<ShellViewModel>().FontOptions;
        }

        /// <summary>
        /// Gets the app font sizes.
        /// </summary>
        public FontOptions FontOptions { get; }
    }
}
