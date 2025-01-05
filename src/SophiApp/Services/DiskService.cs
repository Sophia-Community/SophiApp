// <copyright file="DiskService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using System.Collections.Generic;

    /// <inheritdoc/>
    public class DiskService : IDiskService
    {
        /// <inheritdoc/>
        public IEnumerable<string> GetVolumeLabels()
        {
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    yield return drive.VolumeLabel;
                }
            }
        }
    }
}
