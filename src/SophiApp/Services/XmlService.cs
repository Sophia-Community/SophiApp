// <copyright file="XmlService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Xml;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class XmlService : IXmlService
    {
        /// <inheritdoc/>
        public XmlDocument? TryLoad(string? path)
        {
            if (File.Exists(path))
            {
                var document = new XmlDocument();

                try
                {
                    document.Load(path);
                    return document;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
