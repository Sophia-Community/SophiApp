// <copyright file="NewBadge.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Models;

    /// <summary>
    /// The mark "New" of a setting card. It shows itself when the setting of the card is new in this version of the app.
    /// </summary>
    public sealed partial class NewBadge : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewBadge"/> class.
        /// </summary>
        public NewBadge()
        {
            InitializeComponent();
            DataContextChanged += (_, args) => Visibility = args.NewValue is UIModel { IsNew: true } ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
