using Microsoft.Win32;
using SophiApp.Dto;
using System;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class VisualRedistrLibsHelper
    {
        private const string CLOUD_VC_VERSION_URL = "https://raw.githubusercontent.com/aaronparker/vcredist/main/VcRedist/VisualCRedistributables.json";
        private const string REDISTR_LIB_VS_2022_NAME = "Visual C++ Redistributable for Visual Studio 2022";
        private const string MSREDISTR_LIB_VS_2022_NAME = "Microsoft Visual C++ 2015-2022 Redistributable (x64)";
        private const string REDISTRX64_REGISTRY_NAME_PATTERN = "VC,redist.x64,amd64";
        private const string REDISTRX64_REGISTRY_PATH = @"Installer\Dependencies";
        private const string VERSION_NAME = "Version";
        private const string DISPLAY_NAME = "DisplayName";
        private const string X64 = "x64";

        internal static Version GetCloudLatestVersion()
        {
            var cloudLibsData = WebHelper.GetJsonResponse<CPPRedistrCollection>(CLOUD_VC_VERSION_URL);
            return cloudLibsData.Supported.First(libs => libs.Name == REDISTR_LIB_VS_2022_NAME && libs.Architecture == X64).Version;
        }

        internal static Version GetInstalledVersion()
        {
            var version = IsInstalled() ? GetRegistryPropertyValue(VERSION_NAME) : "0.0.0.0";
            return Version.Parse(version);
        }

        private static string GetRegistryPropertyValue(string propertyName)
        {
            var registryData = RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, REDISTRX64_REGISTRY_PATH)
                                        .FirstOrDefault(key => key.Contains(REDISTRX64_REGISTRY_NAME_PATTERN));

            return RegHelper.GetValue(RegistryHive.ClassesRoot, registryData, propertyName) as string;

        }

        internal static bool IsInstalled()
        {
            var vcRegistryPath = RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, REDISTRX64_REGISTRY_PATH)
                                          .FirstOrDefault(key => key.Contains(REDISTRX64_REGISTRY_NAME_PATTERN));

            return vcRegistryPath != null && RegHelper.GetStringValue(RegistryHive.ClassesRoot, vcRegistryPath, DISPLAY_NAME)
                                                      .Contains(MSREDISTR_LIB_VS_2022_NAME);
        }
    }
}