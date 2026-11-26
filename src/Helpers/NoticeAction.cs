// <copyright file="NoticeAction.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// A button of the notice under a setting.
    /// </summary>
    /// <param name="ResourceKey">Localized string resource key of the button text.</param>
    /// <param name="Address">What the button opens: a page of Windows settings, a folder or a link.</param>
    public sealed record NoticeAction(string ResourceKey, string Address);
}
