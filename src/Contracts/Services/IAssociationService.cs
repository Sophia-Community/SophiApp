// <copyright file="IAssociationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with the default apps of file extensions and protocols.
    /// </summary>
    public interface IAssociationService
    {
        /// <summary>
        /// Gets the name of the app that opens the files with an extension, or the links of a protocol.
        /// </summary>
        /// <param name="extension">The extension with the dot, or the name of the protocol.</param>
        /// <returns>The name of the app, or an empty string if there is none.</returns>
        string GetDefaultApp(string extension);

        /// <summary>
        /// Makes programs the default apps of extensions or protocols for the current user,
        /// so that Windows does not ask how to open the files.
        /// </summary>
        /// <param name="records">The apps: a path to a program, a whole command with it, or the ProgId of a registered app for each extension.</param>
        /// <param name="skipCurrent">True to leave an app that is already the default one as it is.</param>
        /// <returns>The extensions whose app could not be set, with the reason.</returns>
        IReadOnlyList<(string Extension, CustomizationException Error)> SetDefaultApps(IReadOnlyList<AssociationRecord> records, bool skipCurrent);

        /// <summary>
        /// Saves the default apps of the current user to a file.
        /// </summary>
        /// <param name="file">The path of the JSON file to write.</param>
        void Export(string file);

        /// <summary>
        /// Reads the default apps saved to a file.
        /// </summary>
        /// <param name="file">The path of the JSON file.</param>
        /// <returns>The default apps of the file.</returns>
        IReadOnlyList<AssociationRecord> ReadExported(string file);
    }
}
