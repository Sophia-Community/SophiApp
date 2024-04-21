// <copyright file="StringExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    /// <summary>
    /// Implements <see cref="string"/> extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the string to enumerated object.
        /// </summary>
        /// <typeparam name="T">Type of enumerated object.</typeparam>
        /// <param name="value">String to convert.</param>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when <paramref name="value"/> is not found in enum.</exception>
        public static T ToEnum<T>(this string value)
        {
            return Enum.IsDefined(typeof(T), value)
                ? (T)Enum.Parse(typeof(T), value)
                : throw new ArgumentOutOfRangeException(paramName: value, message: $"Value: {value} is not found in {typeof(T).Name} enumeration.");
        }

        /// <summary>
        /// Attempts to delete the specified file. Does not throw any exceptions if it fails.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        public static void TryDelete(this string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception)
            {
                // Do nothing.
            }
        }
    }
}
