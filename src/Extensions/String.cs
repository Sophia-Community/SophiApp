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
        /// Attempts to delete the specified file. Does not throw any exceptions if it fails.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        public static void TryDelete(this string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                // Do nothing.
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
