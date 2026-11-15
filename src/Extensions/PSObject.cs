// <copyright file="PSObject.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System.Management.Automation;

    /// <summary>
    /// Implements <see cref="System.Management.Automation.PSObject"/> extensions.
    /// </summary>
    public static class PSObject
    {
        /// <summary>
        /// Gets the property value of the object that a PowerShell cmdlet returned.
        /// </summary>
        /// <typeparam name="T">Property value type.</typeparam>
        /// <param name="source">The object that a PowerShell cmdlet returned.</param>
        /// <param name="name">The property name.</param>
        /// <returns>The property value, or default if the object has no such property or its value cannot be converted to <typeparamref name="T"/>.</returns>
        public static T? GetPropertyValue<T>(this System.Management.Automation.PSObject source, string name)
            => LanguagePrimitives.TryConvertTo<T>(source.Properties[name]?.Value, out var value) ? value : default;
    }
}
