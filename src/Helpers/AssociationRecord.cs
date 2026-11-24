// <copyright file="AssociationRecord.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// A default app as it is kept in the file of exported associations. The file is the same Sophia Script writes and reads.
    /// </summary>
    /// <param name="ProgId">The name the app is registered in Windows with.</param>
    /// <param name="ProgramPath">The command the app opens a file with, or an empty string for a packaged app.</param>
    /// <param name="Extension">The extension with the dot, or the name of the protocol.</param>
    /// <param name="Icon">The icon of the files, or an empty string.</param>
    public sealed record AssociationRecord(string ProgId, string ProgramPath, string Extension, string Icon);
}
