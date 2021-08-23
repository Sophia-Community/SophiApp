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
            Log = new List<string>();
            Log.Add($"Application version: {AppData.Version}");
            Log.Add($"Application launch folder: \"{AppData.StartupFolder}\"");
            Log.Add($"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()}");
            Log.Add($"{OsHelper.GetProductName()} build: {OsHelper.GetVersion()}");
            Log.Add($"Registered organization: {OsHelper.GetRegisteredOrganization()}");
            Log.Add($"Registered owner: {OsHelper.GetRegisteredOwner()}");
            Log.Add($"Computer name: {Environment.MachineName}");
            Log.Add($"Current user: {Environment.UserName}");
            Log.Add($"User domain: {Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName}");
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