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

        public bool Result { get; set; }
        public string Name { get; set; } = Localization.RequirementTest_WinVer_Name;
        public string ErrorDescription { get; set; } = Localization.RequirementTest_WinVer_ErrorDescription;
        public string Url { get; set; } = Localization.RequirementTest_WinVer_Url;

        public void Run()
        {
            try
            {
                Result = (currentVersion.Version.Major == actualVersion.Version.Major) && (currentVersion.Version.Build >= actualVersion.Version.Build);                
            }
            
            catch (Exception)
            {

                Result = false;
            }            
        }
    }
}
