// <copyright file="IndirectString.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Texts that Windows keeps as a reference to a resource of a library, such as "@%SystemRoot%\system32\input.dll,-5000".
    /// </summary>
    internal static class IndirectString
    {
        /// <summary>
        /// Gets the text a reference leads to, in the language of Windows. A text that is not a reference is returned as it is.
        /// </summary>
        /// <param name="text">A reference to a resource, also in the form "$(@library,-number)", or a plain text.</param>
        /// <returns>The text, or an empty string if the reference leads nowhere.</returns>
        public static string Resolve(string? text)
        {
            var reference = (text ?? string.Empty).Trim();

            // Scheduled tasks wrap the reference
            if (reference.StartsWith("$(", StringComparison.Ordinal) && reference.EndsWith(')'))
            {
                reference = reference[2..^1];
            }

            if (!reference.StartsWith('@'))
            {
                return reference;
            }

            var buffer = new StringBuilder(1024);
            return SHLoadIndirectString(reference, buffer, buffer.Capacity, nint.Zero) == 0 ? buffer.ToString() : string.Empty;
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int SHLoadIndirectString(string source, StringBuilder buffer, int bufferLength, nint reserved);
    }
}
