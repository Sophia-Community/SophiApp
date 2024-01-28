// <copyright file="IntExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    /// <summary>
    /// Implements <see cref="int"/> extensions.
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Proportionally increases the <paramref name="value"/> from 0 to 100 using the number of <paramref name="steps"/>.
        /// </summary>
        /// <param name="value">Increasing value.</param>
        /// <param name="steps">Steps to increase.</param>
        public static int Increase(this int value, int steps)
        {
            return (100 / steps) + value;
        }
    }
}
