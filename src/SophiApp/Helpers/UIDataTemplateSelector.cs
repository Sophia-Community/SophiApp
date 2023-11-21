// <copyright file="UIDataTemplateSelector.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Models;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class UIDataTemplateSelector : DataTemplateSelector
    {
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor.

        /// <summary>
        /// Gets or sets <see cref="ControlTemplates.CheckBox"/> template.
        /// </summary>
        public DataTemplate TextCheckBox { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is UICheckBoxModel)
            {
                return TextCheckBox;
            }

            return TextCheckBox;
        }
#pragma warning restore CS8618 // Non-nullable property must contain a non-null value when exiting constructor.
    }
}
