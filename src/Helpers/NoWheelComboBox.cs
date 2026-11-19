// <copyright file="NoWheelComboBox.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Input;

    /// <summary>
    /// A combo box that does not change its value when the mouse wheel is turned over it.
    /// A combo box that has the focus does, and a setting would be chosen by scrolling the page.
    /// </summary>
    public sealed partial class NoWheelComboBox : ComboBox
    {
        /// <inheritdoc/>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            // The wheel is left to the page
        }
    }
}
