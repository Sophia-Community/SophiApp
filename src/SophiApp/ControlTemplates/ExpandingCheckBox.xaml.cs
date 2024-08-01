// <copyright file="ExpandingCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="ExpandingCheckBox"/> element.
    /// </summary>
    public sealed partial class ExpandingCheckBox : UserControl
    {
        // TODO: Is deprecated, del it?

        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(ExpandingCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandingCheckBox"/> class.
        /// </summary>
        public ExpandingCheckBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets <see cref="TextCheckBox"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
