// <copyright file="NoWheelComboBox.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Windows.UI.ViewManagement;

    /// <summary>
    /// A combo box that does not change its value when the mouse wheel is turned over it.
    /// A combo box that has the focus does, and a setting would be chosen by scrolling the page.
    /// </summary>
    public sealed partial class NoWheelComboBox : ComboBox
    {
        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // In high contrast the list keeps the system colors
            if (GetTemplateChild("Popup") is not Popup popup
                || GetTemplateChild("PopupBorder") is not Border list
                || list.Child is not UIElement items
                || new AccessibilitySettings().HighContrast)
            {
                return;
            }

            // The list lies on the acrylic of the desktop, like the menus: the standard backdrop is put under its items,
            // and the color of the app over the backdrop. The backdrop needs a list that is a window of its own
            popup.ShouldConstrainToRootBounds = false;
            list.Child = null;
            var layers = new Grid();
            layers.Children.Add(new SystemBackdropElement { SystemBackdrop = new DesktopAcrylicBackdrop(), CornerRadius = list.CornerRadius });
            layers.Children.Add(new Border { Style = (Style)Application.Current.Resources["PopupTintLayerStyle"] });
            layers.Children.Add(items);
            list.Child = layers;
            list.Background = new SolidColorBrush(Colors.Transparent);
        }

        /// <inheritdoc/>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            // The wheel is left to the page
        }
    }
}
