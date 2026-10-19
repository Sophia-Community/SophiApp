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
        /// Gets or sets <see cref="UIIconCheckBoxModel"/> template.
        /// </summary>
        public DataTemplate IconCheckBox { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="UICheckBoxModel"/> template.
        /// </summary>
        public DataTemplate TextCheckBox { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="UIRadioButtonsGroup2Model"/> template.
        /// </summary>
        public DataTemplate RadioButtonsGroup2 { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="UIRadioButtonsGroup3Model"/> template.
        /// </summary>
        public DataTemplate RadioButtonsGroup3 { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="UIRadioButtonsGroup4Model"/> template.
        /// </summary>
        public DataTemplate RadioButtonsGroup4 { get; set; } = new ();

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var itemType = item.GetType();
            return itemType switch
            {
                var type when type == typeof(UIIconCheckBoxModel) => IconCheckBox,
                var type when type == typeof(UICheckBoxModel) => TextCheckBox,
                var type when type == typeof(UIRadioButtonsGroup2Model) => RadioButtonsGroup2,
                var type when type == typeof(UIRadioButtonsGroup3Model) => RadioButtonsGroup3,
                var type when type == typeof(UIRadioButtonsGroup4Model) => RadioButtonsGroup4,
                _ => throw new TypeAccessException($"Attempt to access method '{nameof(SelectTemplateCore)}' to type '{itemType}' failed")
            };
        }
    }
}
