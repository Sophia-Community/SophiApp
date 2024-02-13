// <copyright file="UserControlExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements <see cref="FrameworkElement"/> extensions.
    /// </summary>
    public static class UserControlExtensions
    {
        /// <summary>
        /// Retrieves an object that has the specified identifier name.
        /// </summary>
        /// <typeparam name="T">Type of requested object. This can be null if no matching object was found in the current XAML namescope.</typeparam>
        /// <param name="userControl">The base element class for Windows Runtime UI objects.</param>
        /// <param name="name">The name of the requested object.</param>
        public static T FindName<T>(this UserControl userControl, string name)
            where T : FrameworkElement
        {
            return (T)userControl.FindName(name);
        }
    }
}
