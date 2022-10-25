using Microsoft.Win32;
using SophiApp.Dto;
using System;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class VisualRedistrLibsHelper
    {
        private const string CLOUD_VC_VERSION_URL = "https://raw.githubusercontent.com/aaronparker/vcredist/main/VcRedist/VisualCRedistributables.json";
        private const string DISPLAY_NAME = "DisplayName";
        private const string MSREDISTR_X64_LIB_VS_2022_NAME = "Microsoft Visual C++ 2015-2022 Redistributable (x64)";
        private const string MSREDISTR_X86_LIB_VS_2022_NAME = "Microsoft Visual C++ 2015-2022 Redistributable (x32)";
        private const string REDISTR_LIB_VS_2022_NAME = "Visual C++ Redistributable for Visual Studio 2022";
        private const string REDISTRX64_REGISTRY_NAME_PATTERN = "VC,redist.x64,amd64,14";
        private const string REDISTRX32_REGISTRY_NAME_PATTERN = "VC,redist.x86,x86,14";
        private const string INSTALLER_DEPENDENCIES_PATH = @"Installer\Dependencies";
        private const string VERSION_NAME = "Version";
        private const string X64 = "x64";
        private const string X86 = "x86";

        internal static Version GetX64CloudLatestVersion()
        {
            var cloudLibsData = WebHelper.GetJsonResponse<CPPRedistrCollection>(CLOUD_VC_VERSION_URL);
            return cloudLibsData.Supported.First(libs => libs.Name == REDISTR_LIB_VS_2022_NAME && libs.Architecture == X64).Version;
        }

        internal static Version GetX86CloudLatestVersion()
        {
            var cloudLibsData = WebHelper.GetJsonResponse<CPPRedistrCollection>(CLOUD_VC_VERSION_URL);
            var a = cloudLibsData.Supported.First(libs => libs.Name == REDISTR_LIB_VS_2022_NAME && libs.Architecture == X86).Version;
            return cloudLibsData.Supported.First(libs => libs.Name == REDISTR_LIB_VS_2022_NAME && libs.Architecture == X86).Version;
        }

        internal static Version GetX64InstalledVersion()
        {
            var registryData = RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, INSTALLER_DEPENDENCIES_PATH)
                                        .First(key => key.Contains(REDISTRX64_REGISTRY_NAME_PATTERN));

            var version = RegHelper.GetValue(RegistryHive.ClassesRoot, registryData, VERSION_NAME) as string;
            return Version.Parse(version);
        }

        internal static Version GetX86InstalledVersion()
        {
            var registryData = RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, INSTALLER_DEPENDENCIES_PATH)
                                        .First(key => key.Contains(REDISTRX32_REGISTRY_NAME_PATTERN));

            var version = RegHelper.GetValue(RegistryHive.ClassesRoot, registryData, VERSION_NAME) as string;
            return Version.Parse(version);
        }

        internal static bool X64IsInstalled()
        {
            try
            {
                return RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, INSTALLER_DEPENDENCIES_PATH)
                                          .First(key => key.Contains(REDISTRX64_REGISTRY_NAME_PATTERN)) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool X86IsInstalled()
        {
            try
            {
                return RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, INSTALLER_DEPENDENCIES_PATH)
                                          .First(key => key.Contains(REDISTRX32_REGISTRY_NAME_PATTERN)) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}