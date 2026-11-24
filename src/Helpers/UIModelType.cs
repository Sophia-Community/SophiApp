// <copyright file="UIModelType.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using SophiApp.Models;
    using System.Text.Json.Serialization;

    /// <summary>
    /// <see cref="UIModelDto"/> type.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<UIModelType>))]
    public enum UIModelType
    {
        /// <summary>
        /// A setting that is turned on or off.
        /// </summary>
        CheckBox,

        /// <summary>
        /// A setting that is turned on or off and has an icon.
        /// </summary>
        IconCheckBox,

        /// <summary>
        /// A choice of one option out of several. The options are as many as there are titles for them.
        /// </summary>
        RadioButtonsGroup,

        /// <summary>
        /// An action without a state: it is chosen with a button and done when the chosen settings are applied.
        /// </summary>
        Action,

        /// <summary>
        /// A choice of one item from a list that is known only when the app runs.
        /// </summary>
        Choice,

        /// <summary>
        /// A choice of any number of options with icons, none of which can be selected.
        /// </summary>
        IconRadioButtons,

        /// <summary>
        /// A package that is installed or updated with a button.
        /// </summary>
        Install,

        /// <summary>
        /// A package that is installed, updated or uninstalled with buttons.
        /// </summary>
        InstallUninstall,

        /// <summary>
        /// The locations of the user folders: a new one can be chosen for each folder.
        /// </summary>
        UserFolders,

        /// <summary>
        /// The default apps of file extensions: the user adds a program for each extension.
        /// </summary>
        Associations,

        /// <summary>
        /// A list of things that are turned on or off one by one. The list is known only when the app runs.
        /// </summary>
        CheckList,

        /// <summary>
        /// A UWP app that can be uninstalled.
        /// </summary>
        UwpApp,
    }
}
