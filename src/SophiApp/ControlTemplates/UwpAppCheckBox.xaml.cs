// <copyright file="UwpAppCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Models;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="UwpAppCheckBox"/> element.
    /// </summary>
    public sealed partial class UwpAppCheckBox : UserControl
    {
        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(UwpAppCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="CommandParameter"/>.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(UIUwpAppModel), typeof(UwpAppCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="UwpAppCheckBox"/> class.
        /// </summary>
        public UwpAppCheckBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets <see cref="UwpAppCheckBox"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="UwpAppCheckBox"/> command parameter.
        /// </summary>
        public UIUwpAppModel CommandParameter
        {
            get => (UIUwpAppModel)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}
