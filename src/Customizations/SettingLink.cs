// <copyright file="SettingLink.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    /// <summary>
    /// A choice in one setting that decides the value of another one, because both write the same place in Windows.
    /// </summary>
    internal sealed class SettingLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingLink"/> class.
        /// </summary>
        /// <param name="setting">The name of the setting that decides.</param>
        /// <param name="choice">The choice in that setting, or null for any choice.</param>
        /// <param name="dependent">The name of the setting whose value is decided.</param>
        /// <param name="value">The value the dependent setting gets.</param>
        public SettingLink(string setting, object? choice, string dependent, object value)
        {
            Setting = setting;
            Choice = choice;
            Dependent = dependent;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the setting that decides.
        /// </summary>
        public string Setting { get; }

        /// <summary>
        /// Gets the choice that decides: true or false for a check box, the number of the option for a list. Null stands for any choice.
        /// </summary>
        public object? Choice { get; }

        /// <summary>
        /// Gets the name of the setting whose value is decided.
        /// </summary>
        public string Dependent { get; }

        /// <summary>
        /// Gets the value the dependent setting gets.
        /// </summary>
        public object Value { get; }
    }
}
