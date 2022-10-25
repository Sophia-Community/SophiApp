using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SophiApp.Helpers
{
    internal enum DotNetRid
    {
        Win_x86,
        Win_x64,
    }

    internal class DotNetHelper
    {
        private static readonly string ENVIRONMENT_PACKAGE_CACHE = Environment.ExpandEnvironmentVariables("%ProgramData%\\Package Cache");

        private static readonly Dictionary<DotNetRid, string> Platform = new Dictionary<DotNetRid, string>()
        {
            {DotNetRid.Win_x86, "win-x86" }, {DotNetRid.Win_x64, "win-x64" }
        };

        internal static bool IsInstalled(Version version, DotNetRid rid)
        {
            var runtime = $"windowsdesktop-runtime-{version}-{Platform[rid]}.exe";
            return Directory.GetFileSystemEntries(ENVIRONMENT_PACKAGE_CACHE, runtime, SearchOption.AllDirectories)
                            .Count() == 1;
        }

        internal static bool IsInstalled(string runtime, DotNetRid rid)
        {
            return Directory.GetFileSystemEntries(ENVIRONMENT_PACKAGE_CACHE, runtime, SearchOption.AllDirectories)
                            .Count() == 1;
        }

        internal static void Uninstall(string runtime)
        {
            var runtimeFile = Directory.GetFileSystemEntries(ENVIRONMENT_PACKAGE_CACHE, runtime, SearchOption.AllDirectories).First();
            ProcessHelper.StartWait(runtimeFile, "/uninstall /passive /norestart");
        }
    }
}