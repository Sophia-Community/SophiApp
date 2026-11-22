// <copyright file="UserFolder.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// A user folder whose location can be changed.
    /// </summary>
    public enum UserFolder
    {
        /// <summary>
        /// The Desktop folder.
        /// </summary>
        Desktop,

        /// <summary>
        /// The Documents folder.
        /// </summary>
        Documents,

        /// <summary>
        /// The Downloads folder.
        /// </summary>
        Downloads,

        /// <summary>
        /// The Music folder.
        /// </summary>
        Music,

        /// <summary>
        /// The Pictures folder.
        /// </summary>
        Pictures,

        /// <summary>
        /// The Videos folder.
        /// </summary>
        Videos,

        /// <summary>
        /// The folder of the screenshots taken with Win+PrtScn or Win+Shift+S.
        /// </summary>
        Screenshots,
    }
}
