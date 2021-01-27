using SophiAppCE.Commons;
using SophiAppCE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SophiAppCE.Requirements
{
    internal class WindowsVersionTest : IRequirementTest
    {
        private readonly OperatingSystem actualVersion = new OperatingSystem(PlatformID.Win32NT, new Version(Constants.WindowsMajor, Constants.WindowsMinor, Constants.WindowsBuild));            
        private readonly OperatingSystem currentVersion = Environment.OSVersion;

        public bool Result { get; set; } = false;
        public string Name { get; set; } = Localization.RequirementTest_WinVer_Name;
        public string Error { get; set; } = Localization.RequirementTest_WinVer_Error;

        public void Run()
        {
            Result = (currentVersion.Version.Major == actualVersion.Version.Major) && (currentVersion.Version.Build >= actualVersion.Version.Build);
            Thread.Sleep(5000);
        }
    }
}
