// <copyright file="StartupCheckAction.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Windows.Input;

    /// <summary>
    /// A button of the notice of a failed startup check.
    /// </summary>
    /// <param name="Text">The text of the button.</param>
    /// <param name="Command">What the button does.</param>
    /// <param name="IsPrimary">Whether it is the answer the app suggests.</param>
    public sealed record StartupCheckAction(string Text, ICommand Command, bool IsPrimary = false);
}
