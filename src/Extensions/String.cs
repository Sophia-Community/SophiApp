// <copyright file="String.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    /// <summary>
    /// Implements <see cref="string"/> extensions.
    /// </summary>
    public static class String
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

        /// <summary>
        /// Performs the specified function on each element of the strings array.
        /// </summary>
        /// <param name="strings">The strings array.</param>
        /// <param name="function">Encapsulates a method that has one parameters and returns a string.</param>
        public static IEnumerable<string> ForEach(this string[] strings, Func<string, string> function)
        {
            foreach (string s in strings)
            {
                yield return function(s);
            }
        }

        /// <summary>
        /// Performs the specified action on each element of the strings array.
        /// </summary>
        /// <param name="strings">The strings array.</param>
        /// <param name="action">Encapsulates a method that has a single parameter and does not return a value.</param>
        public static void ForEach(this string[] strings, Action<string> action)
        {
            foreach (string s in strings)
            {
                action(s);
            }
        }
    }
}
