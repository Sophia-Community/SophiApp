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
        /// A choice of two options.
        /// </summary>
        RadioButtonsGroup2,

        /// <summary>
        /// A choice of three options.
        /// </summary>
        RadioButtonsGroup3,

        /// <summary>
        /// A choice of four options.
        /// </summary>
        RadioButtonsGroup4,

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
        /// A UWP app that can be uninstalled.
        /// </summary>
        UwpApp,
    }
}
