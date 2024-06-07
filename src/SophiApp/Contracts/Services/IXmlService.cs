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
        /// Try load the XML document from the specified path.
        /// </summary>
        /// <param name="path">Path for the file containing the XML document to load.</param>
        XmlDocument? TryLoad(string? path);
    }
}
