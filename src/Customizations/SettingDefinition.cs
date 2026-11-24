// <copyright file="SettingDefinition.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using SophiApp.Helpers;

    /// <summary>
    /// A setting of the app: its name, its place in the UI and the methods that read and change it.
    /// A definition is made by the method named after the kind of the setting, so the compiler checks that the methods fit that kind.
    /// </summary>
    internal sealed class SettingDefinition
    {
        private SettingDefinition(string name, UIModelType type, UICategoryTag tag, Delegate accessor, Delegate mutator, string group, bool windows11, bool windows11Ltsc)
        {
            Name = name;
            Type = type;
            Tag = tag;
            Accessor = accessor;
            Mutator = mutator;
            Group = group;
            Windows11 = windows11;
            Windows11LTSC = windows11Ltsc;
        }

        /// <summary>
        /// Gets the name of the setting. The keys of its texts are built from it.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the kind of the setting.
        /// </summary>
        public UIModelType Type { get; }

        /// <summary>
        /// Gets the page of the setting.
        /// </summary>
        public UICategoryTag Tag { get; }

        /// <summary>
        /// Gets the method that reads the setting from Windows.
        /// </summary>
        public Delegate Accessor { get; }

        /// <summary>
        /// Gets the method that changes the setting in Windows.
        /// </summary>
        public Delegate Mutator { get; }

        /// <summary>
        /// Gets the group of settings on the page, or an empty string.
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Gets a value indicating whether the setting is available on Windows 11.
        /// </summary>
        public bool Windows11 { get; }

        /// <summary>
        /// Gets a value indicating whether the setting is available on Windows 11 LTSC.
        /// </summary>
        public bool Windows11LTSC { get; }

        /// <summary>
        /// Gets the index of the icon of the setting.
        /// </summary>
        public int IconIndex { get; private init; }

        /// <summary>
        /// Gets a value indicating whether the options of the setting have icons.
        /// </summary>
        public bool OptionIcons { get; private init; }

        /// <summary>
        /// Gets a value indicating whether the package can be uninstalled.
        /// </summary>
        public bool CanUninstall { get; private init; }

        /// <summary>
        /// Gets the method that installs or uninstalls the package for all users, or null.
        /// </summary>
        public Action<bool>? AllUsersMutator { get; private init; }

        /// <summary>
        /// Gets the method that returns the resource key of a notice for the current state of the system, or null.
        /// </summary>
        public Func<string?>? Notice { get; private init; }

        /// <summary>
        /// Gets the method that tells what Windows is set to when it is none of the options, or null.
        /// </summary>
        public Func<ThirdPartyState>? ThirdParty { get; private init; }

        /// <summary>
        /// Gets the method that reads the default apps saved to a file, or null.
        /// </summary>
        public Func<string, IReadOnlyList<AssociationRecord>>? ImportReader { get; private init; }

        /// <summary>
        /// Gets the method that sets the default apps read from a file and returns the extensions it could not set, or null.
        /// </summary>
        public Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>>? Importer { get; private init; }

        /// <summary>
        /// Gets the method that saves the default apps to a file, or null.
        /// </summary>
        public Action<string>? Exporter { get; private init; }

        /// <summary>
        /// A setting that is turned on or off.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that reads the setting.</param>
        /// <param name="mutator">The method that changes the setting.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="notice">The method that returns the resource key of a notice.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition CheckBox(string name, UICategoryTag tag, Func<bool> accessor, Action<bool> mutator, string group = "", Func<string?>? notice = null, bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.CheckBox, tag, accessor, mutator, group, windows11, windows11Ltsc) { Notice = notice };

        /// <summary>
        /// A setting that is turned on or off and has an icon.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that reads the setting.</param>
        /// <param name="mutator">The method that changes the setting.</param>
        /// <param name="iconIndex">The index of the icon.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition IconCheckBox(string name, UICategoryTag tag, Func<bool> accessor, Action<bool> mutator, int iconIndex, string group = "", bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.IconCheckBox, tag, accessor, mutator, group, windows11, windows11Ltsc) { IconIndex = iconIndex };

        /// <summary>
        /// A choice of one option out of several. The options are numbered from 1.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the number of the selected option.</param>
        /// <param name="mutator">The method that sets the option with the given number.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="optionIcons">The options have icons.</param>
        /// <param name="thirdParty">The method that tells what Windows is set to when the accessor returns <see cref="ThirdPartyState.Id"/>.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition RadioButtons(string name, UICategoryTag tag, Func<int> accessor, Action<int> mutator, string group = "", bool optionIcons = false, Func<ThirdPartyState>? thirdParty = null, bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.RadioButtonsGroup, tag, accessor, mutator, group, windows11, windows11Ltsc) { OptionIcons = optionIcons, ThirdParty = thirdParty };

        /// <summary>
        /// A choice of any number of options with icons, none of which can be selected.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the number of the selected option, or 0.</param>
        /// <param name="mutator">The method that sets the option with the given number.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="thirdParty">The method that tells what Windows is set to when the accessor returns <see cref="ThirdPartyState.Id"/>.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition IconRadioButtons(string name, UICategoryTag tag, Func<int> accessor, Action<int> mutator, string group = "", Func<ThirdPartyState>? thirdParty = null, bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.IconRadioButtons, tag, accessor, mutator, group, windows11, windows11Ltsc) { ThirdParty = thirdParty };

        /// <summary>
        /// A package that is installed, updated or uninstalled with buttons.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the state of the package.</param>
        /// <param name="mutator">The method that installs or uninstalls the package.</param>
        /// <param name="iconIndex">The index of the icon.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="canUninstall">The package can be uninstalled.</param>
        /// <param name="allUsersMutator">The method that installs or uninstalls the package for all users.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition Install(string name, UICategoryTag tag, Func<InstallState> accessor, Action<bool> mutator, int iconIndex, string group = "", bool canUninstall = false, Action<bool>? allUsersMutator = null, bool windows11 = true, bool windows11Ltsc = true)
            => new(name, canUninstall ? UIModelType.InstallUninstall : UIModelType.Install, tag, accessor, mutator, group, windows11, windows11Ltsc)
            {
                IconIndex = iconIndex,
                CanUninstall = canUninstall,
                AllUsersMutator = allUsersMutator,
            };

        /// <summary>
        /// An action without a state: it is chosen with a button and done when the chosen settings are applied.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that tells whether the action is available.</param>
        /// <param name="mutator">The method that does the action.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition ActionButton(string name, UICategoryTag tag, Func<bool> accessor, Action<bool> mutator, string group = "", bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.Action, tag, accessor, mutator, group, windows11, windows11Ltsc);

        /// <summary>
        /// A choice of one item from a list that is known only when the app runs.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the items to choose from.</param>
        /// <param name="mutator">The method that applies the id of the chosen item.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="iconIndex">The index of the icon, or -1 if the setting has no icon.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition Choice(string name, UICategoryTag tag, Func<IReadOnlyList<SettingChoice>> accessor, Action<string> mutator, string group = "", int iconIndex = -1, bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.Choice, tag, accessor, mutator, group, windows11, windows11Ltsc) { IconIndex = iconIndex };

        /// <summary>
        /// A list of things that are turned on or off one by one. The list is known only when the app runs.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the items with their state.</param>
        /// <param name="mutator">The method that applies the new state of the changed items, by their ids.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition CheckList(string name, UICategoryTag tag, Func<IReadOnlyList<SettingCheckItem>> accessor, Action<IReadOnlyDictionary<string, bool>> mutator, string group = "", bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.CheckList, tag, accessor, mutator, group, windows11, windows11Ltsc);

        /// <summary>
        /// The locations of the user folders: a new one can be chosen for each folder.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the location of a folder.</param>
        /// <param name="mutator">The method that changes the location of a folder.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition UserFolders(string name, UICategoryTag tag, Func<UserFolder, string> accessor, Action<UserFolder, string> mutator, string group = "", bool windows11 = true, bool windows11Ltsc = true)
            => new(name, UIModelType.UserFolders, tag, accessor, mutator, group, windows11, windows11Ltsc);

        /// <summary>
        /// The default apps of file extensions and protocols: the user adds a program for each of them.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="tag">The page of the setting.</param>
        /// <param name="accessor">The method that returns the name of the app that opens an extension.</param>
        /// <param name="mutator">The method that makes programs the default apps of extensions and returns the reasons of the failures.</param>
        /// <param name="importReader">The method that reads the default apps saved to a file.</param>
        /// <param name="importer">The method that sets the default apps read from a file and returns the extensions it could not set.</param>
        /// <param name="exporter">The method that saves the default apps to a file.</param>
        /// <param name="group">The group of settings on the page.</param>
        /// <param name="windows11">The setting is available on Windows 11.</param>
        /// <param name="windows11Ltsc">The setting is available on Windows 11 LTSC.</param>
        public static SettingDefinition Associations(
            string name,
            UICategoryTag tag,
            Func<string, string> accessor,
            Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>> mutator,
            Func<string, IReadOnlyList<AssociationRecord>> importReader,
            Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>> importer,
            Action<string> exporter,
            string group = "",
            bool windows11 = true,
            bool windows11Ltsc = true)
            => new(name, UIModelType.Associations, tag, accessor, mutator, group, windows11, windows11Ltsc)
            {
                ImportReader = importReader,
                Importer = importer,
                Exporter = exporter,
            };
    }
}
