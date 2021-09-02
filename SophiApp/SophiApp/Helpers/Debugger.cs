using System;
using System.Collections.Generic;
using System.IO;

namespace SophiApp.Helpers
{
    internal class Debugger
    {
        private List<string> Log;

        public Debugger()
        {
            Log = new List<string>
            {
                $"Application version: {AppData.Version}",
                $"Application launch folder: \"{AppData.StartupFolder}\"",
                $"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()}",
                $"{OsHelper.GetProductName()} build: {OsHelper.GetVersion()}",
                $"Registered organization: {OsHelper.GetRegisteredOrganization()}",
                $"Registered owner: {OsHelper.GetRegisteredOwner()}",
                $"Computer name: {Environment.MachineName}",
                $"Current user: {Environment.UserName}",
                $"User domain: {Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName}"
            };
        }

        public Debugger(string language, string theme) : this()
        {
            Log.Add($"Application localization: {language}");
            Log.Add($"Application theme: {theme}");
            Log.Add(string.Empty);
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