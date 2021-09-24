using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SophiApp.Helpers
{
    internal class DebugHelper
    {
        private List<string> ErrorLog = new List<string>();
        private List<string> InfoLog;
        private List<string> StatusLog = new List<string>();
        private List<string> UpdateLog = new List<string>();

        public DebugHelper(string language, string theme)
        {
            InfoLog = new List<string>
            {
                $"Application version: {DataHelper.Version}",
                $"Application launch folder: \"{DataHelper.StartupFolder}\"",
                $"Application localization: {language}",
                $"Application theme: {theme}",
                $"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()} build: {OsHelper.GetVersion()}",
                $"Registered organization: {OsHelper.GetRegisteredOrganization()}",
                $"Registered owner: {OsHelper.GetRegisteredOwner()}",
                $"Computer name: {Environment.MachineName}",
                $"Current user: {Environment.UserName}",
                $"User domain: {Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName}",
                $"User culture: {OsHelper.GetCurrentCultureName()}",
                $"User region: {OsHelper.GetRegionName()}"
            };
        }

        private string GetDateTimeString()
        {
            var dateTime = DateTime.Now;
            return $"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t";
        }

        internal void ActionEntry(uint id, bool parameter) => StatusEntry($"Customization action {id} with parameter {parameter} completed successfully");

        internal void ElementChanged(uint id, ElementStatus status) => StatusEntry($"The element {id} has changed status to: {status}");

        internal void Exception(string message, Exception e)
        {
            var dateTime = GetDateTimeString();
            ErrorLog.AddRange(new List<string>()
            {
                $"{dateTime}{message}",
                $"{dateTime}Error information: {e.Message}",
                $"{dateTime}The class that caused the error: {e.TargetSite.DeclaringType.FullName}",
                $"{dateTime}The method that caused the error: {e.TargetSite.Name}"
            });
        }

        internal void HasRelease(string version, bool prerelease, bool draft) => UpdateLog.AddRange(new List<string>()
        {
            $"New version {version} is available",
            $"Version {version} is prerelease: {prerelease}",
            $"Version {version} is draft: {draft}"
        });

        internal void Save(string path) => File.WriteAllLines(path, new List<string>().Merge(InfoLog).Merge(UpdateLog).Split(string.Empty)
                                                                                      .Merge(ErrorLog).Split(string.Empty).Merge(StatusLog)
                                                                                      .Split(string.Empty));

        internal void StatusEntry(string record) => StatusLog.Add($"{GetDateTimeString()}{record}");

        internal void StopApplying(Stopwatch stopwatch) => StatusEntry($"It took {string.Format("{0:N0}", stopwatch.Elapsed.TotalSeconds)} seconds to apply the setting(s)");

        internal void StopInit(Stopwatch stopwatch) => StatusEntry($"The collection initialization took {string.Format("{0:N0}", stopwatch.Elapsed.TotalSeconds)} seconds");

        internal void UpdateEntry(string record) => UpdateLog.Add(record);

        internal void UpdateResponseIsNull(bool isNull) => UpdateEntry(isNull ? "When checking for an update, no response was received from the update server"
                                                                            : "When checking for an update, a response was received from the update server");
    }
}