// <copyright file="AssociationHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.Win32;
    using SophiApp.Extensions;

    /// <summary>
    /// Reads and checks what the user typed to make a program the default app of an extension or a protocol.
    /// </summary>
    public static class AssociationHelper
    {
        private const string ExecutableExtension = ".exe";

        // How a command gets the file to open
        private static readonly string[] FilePlaceholders = ["%1", "%L", "%*"];

        /// <summary>
        /// Splits what is typed as a program. It is a path to an executable file, a whole command with it
        /// such as <c>"C:\App\app.exe" --open "%1"</c>, or a ProgId: the name an app is registered in Windows with.
        /// </summary>
        /// <param name="program">What is typed as a program.</param>
        /// <returns>The path to the file or the ProgId; the whole command if there is one; whether it is a file.</returns>
        public static (string Target, string? Command, bool IsFile) ParseProgram(string program)
        {
            var text = Environment.ExpandEnvironmentVariables(program.Trim());

            // A path has a drive, a ProgId has no colon
            if (!text.Contains(':'))
            {
                return (text, null, false);
            }

            var end = text.IndexOf(ExecutableExtension, StringComparison.OrdinalIgnoreCase);

            if (end < 0)
            {
                return (text.Trim('"'), null, true);
            }

            // The arguments of a command are kept only if the file to open is among them: it is passed as %1, %L or %*
            var arguments = text[(end + ExecutableExtension.Length)..];
            var command = FilePlaceholders.Any(placeholder => arguments.Contains(placeholder, StringComparison.OrdinalIgnoreCase)) ? text : null;
            return (text[..(end + ExecutableExtension.Length)].Trim().Trim('"'), command, true);
        }

        /// <summary>
        /// Gets a value indicating whether the text is an extension, not a protocol.
        /// </summary>
        /// <param name="extension">An extension with the dot, or the name of a protocol.</param>
        public static bool IsFileExtension(string extension) => extension.Contains('.');

        /// <summary>
        /// Gets the path of the file an icon is taken from: "path,index" without the index.
        /// </summary>
        /// <param name="icon">The path to an icon file, or to a file with icons and the number of the icon.</param>
        public static string GetIconFile(string icon)
        {
            var path = Environment.ExpandEnvironmentVariables(icon.Trim());

            if (!path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) && path.Contains(','))
            {
                path = path[..path.LastIndexOf(',')];
            }

            return path.Trim().Trim('"');
        }

        /// <summary>
        /// Tells why a program cannot be made the default app, or null if it can.
        /// </summary>
        /// <param name="program">A path to a program, a command or a ProgId.</param>
        /// <param name="extension">An extension with the dot, or the name of a protocol.</param>
        /// <param name="icon">A path to an icon, or an empty string.</param>
        /// <returns>The resource key of the reason and the value for its placeholder.</returns>
        public static (string ResourceKey, string Argument)? GetProblem(string program, string extension, string icon)
        {
            var (target, _, isFile) = ParseProgram(program);

            if (isFile && !File.Exists(target))
            {
                return ("UIModel_SetAssociation_ProgramNotFound", target);
            }

            if (!isFile && target.Length > 0 && !Registry.ClassesRoot.SubKeyExists(target))
            {
                return ("UIModel_SetAssociation_ProgIdNotFound", target);
            }

            if (extension.Length > 0 && !IsExtensionValid(extension))
            {
                return ("UIModel_SetAssociation_ExtensionNotValid", extension);
            }

            if (icon.Trim().Length > 0 && GetIconFile(icon) is var iconFile && !File.Exists(iconFile))
            {
                return ("UIModel_SetAssociation_IconNotFound", iconFile);
            }

            return null;
        }

        // An extension starts with a dot; a word without a dot has to be a protocol Windows knows, such as "http"
        private static bool IsExtensionValid(string extension)
        {
            if (extension.AsSpan().IndexOfAny(" \\/:*?\"<>|") >= 0)
            {
                return false;
            }

            return extension.StartsWith('.')
                ? extension.Length > 1
                : Registry.ClassesRoot.GetSubKeyValue(extension, "URL Protocol") is not null;
        }
    }
}
