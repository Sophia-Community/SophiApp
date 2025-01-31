// <copyright file="SquareCheckBox.xaml.cs" company="Team Sophia">
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
    /// Implements the logic and appearance of the <see cref="SquareCheckBox"/> element.
    /// </summary>
    public sealed partial class SquareCheckBox : UserControl
    {
        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(SquareCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareCheckBox"/> class.
        /// </summary>
        public SquareCheckBox()
        {
            // TODO: Not used?
            this.InitializeComponent();
            FontOptions = App.GetService<ShellViewModel>().FontOptions;
        }

        /// <summary>
        /// Gets the app font sizes.
        /// </summary>
        public FontOptions FontOptions { get; }

        /// <summary>
        /// Gets or sets <see cref="SquareCheckBox"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
