// <copyright file="StringExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System.Diagnostics;
    using System.ServiceProcess;

    /// <summary>
    /// Implements <see cref="string"/> extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Invoke the string as a cmd command.
        /// </summary>
        /// <param name="command">String command to be executed.</param>
        public static Process InvokeAsCmd(this string command)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}";
            _ = process.Start();
            process.WaitForExit();
            return process;
        }

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
        /// Determines whether the specified service exists.
        /// </summary>
        /// <param name="service">Service name.</param>
        public static bool ServiceExist(this string service)
        {
            try
            {
                var serviceController = new ServiceController(service);
                return serviceController.ServiceName.Equals(service);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
