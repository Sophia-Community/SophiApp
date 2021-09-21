using System;
using System.Collections.Generic;
using System.IO;

namespace SophiApp.Helpers
{
    internal class DebugHelper
    {
        private List<string> Log;

        public DebugHelper(string language, string theme)
        {
            Log = new List<string>
            {
                $"Application version: {AppData.Version}",
                $"Application launch folder: \"{AppData.StartupFolder}\"",
                $"Application localization: {language}",
                $"Application theme: {theme}",
                $"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()} build: {OsHelper.GetVersion()}",
                $"Registered organization: {OsHelper.GetRegisteredOrganization()}",
                $"Registered owner: {OsHelper.GetRegisteredOwner()}",
                $"Computer name: {Environment.MachineName}",
                $"Current user: {Environment.UserName}",
                $"User domain: {Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName}",
                $"User culture: {OsHelper.GetCurrentCultureName()}",
                $"User region: {OsHelper.GetRegionName()}",
                string.Empty
            };
        }

        private string GetDateTime()
        {
            var dateTime = DateTime.Now;
            return $"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t";
        }

        internal void AddRecord(string record) => Log.Add($"{GetDateTime()}{record}");

        internal void Save(string path) => File.WriteAllLines(path, Log);
    }
}