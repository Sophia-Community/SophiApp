// <copyright file="StringExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System.Collections.ObjectModel;
    using System.Management.Automation;

    /// <summary>
    /// Implements <see cref="string"/> extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Invoke the string as a PowerShell script.
        /// </summary>
        /// <param name="script">String to be executed.</param>
        public static Collection<PSObject> InvokeAsPoweShell(this string script)
        {
            return PowerShell.Create()
                .AddScript(script)
                .Invoke();
        }
    }
}
