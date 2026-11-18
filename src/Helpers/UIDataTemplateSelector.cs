// <copyright file="UIDataTemplateSelector.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Models;

    /// <inheritdoc/>
    public class UIDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for <see cref="UICheckBoxModel"/> and <see cref="UIIconCheckBoxModel"/>.
        /// </summary>
        public DataTemplate CheckBoxTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for radio button groups of any size.
        /// </summary>
        public DataTemplate RadioButtonsTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for <see cref="UIInstallModel"/>.
        /// </summary>
        public DataTemplate InstallTemplate { get; set; } = new();

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            UICheckBoxModel or UIIconCheckBoxModel => CheckBoxTemplate,
            UIInstallModel => InstallTemplate,
            UIRadioButtonsGroup2Model or UIRadioButtonsGroup3Model or UIRadioButtonsGroup4Model => RadioButtonsTemplate,
            _ => throw new TypeAccessException($"Attempt to access method '{nameof(SelectTemplateCore)}' to type '{item.GetType()}' failed"),
        };
    }
}
