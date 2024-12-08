// <copyright file="ExpandingRadioGroup.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="ExpandingRadioGroup"/> element.
    /// </summary>
    public sealed partial class ExpandingRadioGroup : UserControl
    {
        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(ExpandingRadioGroup), new PropertyMetadata(default));

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

        /// <summary>
        /// Gets or sets <see cref="ExpandingRadioGroup"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
