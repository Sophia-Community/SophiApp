// <copyright file="DoubleExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    /// <summary>
    /// Implements <see cref="double"/> extensions.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Proportionally increases the <paramref name="value"/> using <paramref name="step"/>.
        /// </summary>
        /// <param name="value">Increasing value.</param>
        /// <param name="step">Value increment step.</param>
        public static double PartialIncrease(this double value, int step)
        {
            return Math.Round((1.0 / step) + value, 2);
        }
    }
}
