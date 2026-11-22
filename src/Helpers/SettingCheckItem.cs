// <copyright file="SettingCheckItem.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// An item of a setting that is a list of things turned on or off one by one, such as scheduled tasks.
    /// </summary>
    /// <param name="Id">The id the item is changed by.</param>
    /// <param name="Title">The name of the item.</param>
    /// <param name="Subtitle">The second line under the name, or an empty string.</param>
    /// <param name="IsChecked">True if the item is turned on in Windows.</param>
    public sealed record SettingCheckItem(string Id, string Title, string Subtitle, bool IsChecked);
}
