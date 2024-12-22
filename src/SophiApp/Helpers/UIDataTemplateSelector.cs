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
        /// Gets or sets <see cref="TextCheckBox"/> template.
        /// </summary>
        public DataTemplate TextCheckBox { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="ExpandingRadioGroup"/> template.
        /// </summary>
        public DataTemplate ExpandingRadioGroup { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="ExpandingCheckBoxGroup"/> template.
        /// </summary>
        public DataTemplate ExpandingCheckBoxGroup { get; set; } = new ();

        /// <summary>
        /// Gets or sets <see cref="ExpandingCheckBox"/> template.
        /// </summary>
        public DataTemplate ExpandingCheckBox { get; set; } = new (); // TODO: Its unused?

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var itemType = item.GetType();
            return itemType switch
            {
                var type when type == typeof(UICheckBoxModel) => TextCheckBox,
                var type when type == typeof(UIExpandingRadioGroupModel) => ExpandingRadioGroup,
                var type when type == typeof(UIExpandingCheckBoxModel) => ExpandingCheckBox,
                _ => throw new TypeAccessException($"Attempt to access method '{nameof(SelectTemplateCore)}' to type '{itemType}' failed")
            };
        }
    }
}
