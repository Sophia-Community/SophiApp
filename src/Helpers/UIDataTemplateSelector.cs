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
        /// Gets or sets the template for the choices of one option out of any number.
        /// </summary>
        public DataTemplate ComboBoxTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for <see cref="UIInstallModel"/>.
        /// </summary>
        public DataTemplate InstallTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for <see cref="UIActionModel"/>.
        /// </summary>
        public DataTemplate ActionTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for <see cref="UIUserFoldersModel"/>.
        /// </summary>
        public DataTemplate UserFoldersTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for <see cref="UIAssociationsModel"/>.
        /// </summary>
        public DataTemplate AssociationsTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for <see cref="UICheckListModel"/>.
        /// </summary>
        public DataTemplate CheckListTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for the subtitle of a group of settings.
        /// </summary>
        public DataTemplate GroupHeaderTemplate { get; set; } = new();

        /// <summary>
        /// Gets or sets the template for the subtitle that opens the page: it needs less room above.
        /// </summary>
        public DataTemplate FirstGroupHeaderTemplate { get; set; } = new();

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            SettingsGroupHeader { IsFirst: true } => FirstGroupHeaderTemplate,
            SettingsGroupHeader => GroupHeaderTemplate,
            UIUserFoldersModel => UserFoldersTemplate,
            UIAssociationsModel => AssociationsTemplate,
            UICheckListModel => CheckListTemplate,
            UICheckBoxModel or UIIconCheckBoxModel => CheckBoxTemplate,
            UIActionModel => ActionTemplate,
            UIInstallModel => InstallTemplate,
            UIRadioButtonsGroupModel or UIIconRadioButtonsModel or UIChoiceModel => ComboBoxTemplate,
            _ => throw new TypeAccessException($"Attempt to access method '{nameof(SelectTemplateCore)}' to type '{item.GetType()}' failed"),
        };
    }
}
