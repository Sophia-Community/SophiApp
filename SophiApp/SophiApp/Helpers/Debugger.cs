using System;
using System.Collections.Generic;
using System.IO;

namespace SophiApp.Helpers
{
    internal class Debugger
    {
        private readonly Dictionary<string, string> Records = new Dictionary<string, string>()
        {
            {"APP_VERSION", "Application version"}, {"STARTUP_DIR", "Application is launched from the folder"}, {"OS_NAME", "OS name"},
            {"OS_BUILD", "OS build"}, {"OS_ORG", "Registered organization is"}, {"OS_OWNER", "Registered owner is"},
            {"COMP_NAME", "Computer name is"}, {"USER_NAME", "User name is"}, {"USER_DOMAIN", "User domain is"},
            {"HYPERLINK_OPEN", "User clicked on the link"}, {"LOCALIZATION", "Application localization is"}, {"THEME", "Application theme is"},
            {"INIT_ELEMENTS", "Start initialization texted elements"}, {"INIT_ELEMENTS_DONE", "Completing the initialization of texted elements"},
            {"ELEMENT_HAS_ERROR", "An error occured in element numbered is"}, {"ERROR_MESSAGE", "Error information: "},
            {"ERROR_CLASS", "The class and method that caused the error"}, {"ELEMENT_CHANGE_STATUS", "The item has changed its status"}
        };

        private List<string> Log = new List<string>();

        public Debugger()
        {
            Init();
        }

        private void Init()
        {
            InitWrite("APP_VERSION", $"{AppData.Version}");
            InitWrite("STARTUP_DIR", $"{AppData.StartupFolder}");
            InitWrite("OS_NAME", $"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()}");
            InitWrite("OS_BUILD", $"{Environment.OSVersion.VersionString}");
            InitWrite("OS_ORG", $"{OsHelper.GetRegisteredOrganization()}");
            InitWrite("OS_ORG", $"{OsHelper.GetRegisteredOrganization()}");
            InitWrite("OS_OWNER", $"{OsHelper.GetRegisteredOwner()}");
            InitWrite("COMP_NAME", $"{Environment.MachineName}");
            InitWrite("USER_NAME", $"{Environment.UserName}");
            InitWrite("USER_DOMAIN", $"{Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName}");
        }

        internal void InitWrite(string record, string data) => Log.Add($"{Records[record]} {data}");

        internal void InitWrite() => Log.Add(string.Empty);

        internal void Save(string path) => File.WriteAllLines(path, Log);

        internal void Write(string record)
        {
            var dateTime = DateTime.Now;
            Log.Add($"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t{Records[record]}");
        }

        internal void Write(string record, string value)
        {
            var dateTime = DateTime.Now;
            Log.Add($"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t{Records[record]} {value}");
        }

        internal void Write(string record1, string value1, string record2, string value2, string record3, string value3)
        {
            var dateTime = DateTime.Now;
            Log.Add(string.Empty);
            Log.Add($"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t{Records[record1]} {value1}");
            Log.Add($"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t{Records[record2]} {value2}");
            Log.Add($"{dateTime.ToShortDateString()}\t{dateTime.ToLongTimeString()}\t{Records[record3]} {value3}");
            Log.Add(string.Empty);
        }
    }
}