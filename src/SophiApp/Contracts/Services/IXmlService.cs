// <copyright file="IXmlService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Xml;

    /// <summary>
    ///  A service for working with <see cref="XmlDocument"/> API.
    /// </summary>
    public interface IXmlService
    {
        /// <summary>
        /// Get scheduled task arguments XML node or null.
        /// </summary>
        /// <param name="path">Scheduled task xml file path.</param>
        XmlNode? GetScheduledTaskArguments(string path);

        /// <summary>
        /// Try load the XML document from the specified path.
        /// </summary>
        /// <param name="path">Path for the file containing the XML document to load.</param>
        XmlDocument? TryLoad(string path);
    }
}
