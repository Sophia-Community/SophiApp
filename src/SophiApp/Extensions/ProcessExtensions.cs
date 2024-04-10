// <copyright file="ProcessExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    /// <summary>
    /// Implements <see cref="System.Diagnostics.Process"/> extensions.
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the process collection.
        /// </summary>
        /// <param name="processes"><see cref="System.Diagnostics.Process"/> collection.</param>
        /// <param name="action">Encapsulates a method that has a single parameter and does not return a value.</param>
        public static void ForEach(this System.Diagnostics.Process[] processes, Action<System.Diagnostics.Process> action)
        {
            foreach (var process in processes)
            {
                action(process);
            }
        }
    }
}
