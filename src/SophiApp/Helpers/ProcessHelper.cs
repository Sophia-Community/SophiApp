// <copyright file="ProcessHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Diagnostics;
    using SophiApp.Extensions;

    /// <summary>
    /// A class for working with <see cref="Process"/>.
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// Interrupts process operation.
        /// </summary>
        /// <param name="name">Process name.</param>
        /// <param name="timeout">Time, in milliseconds, to wait for the process to complete.</param>
        public static void Stop(string name, int timeout = 1000)
        {
            Process.GetProcessesByName(name)
                .ForEach(process =>
                {
                    process.Kill();
                    process.WaitForExit(timeout);
                    process.Dispose();
                });
        }
    }
}
