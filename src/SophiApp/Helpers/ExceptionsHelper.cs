using System;

namespace SophiApp.Helpers
{
    internal class AdapterTypeInternalOrNullException : Exception
    {
        public AdapterTypeInternalOrNullException(string dacType) : base($"Video adapter DAC type should not be a internal or null. You adapter type is: {dacType}")
        {
        }
    }

    internal class ApplicationBlockedByPolicyException : Exception
    {
        public ApplicationBlockedByPolicyException(string name) : base($"Application {name} blocked by policy")
        {
        }
    }

    internal class BitlockerIsEnabledException : Exception
    {
        public BitlockerIsEnabledException() : base("BitLocker protection is enabled")
        {
        }
    }

    internal class DotNetInstalledException : Exception
    {
        public DotNetInstalledException(Version version) : base($".Net version {version} already installed on this PC")
        {
        }
    }

    internal class DotNetNotInstalledException : Exception
    {
        public DotNetNotInstalledException(Version version) : base($".Net version {version} is not installed on this PC")
        {
        }
    }

    internal class FileNotExistException : Exception
    {
        public FileNotExistException(string filePath) : base($"File does not exist \"{filePath}\"")
        {
        }
    }

    internal class MicrosoftDefenderDisabledByGroupPolicy : Exception
    {
        public MicrosoftDefenderDisabledByGroupPolicy() : base("Microsoft Defender disabled by group policy")
        {
        }
    }

    internal class MicrosoftDefenderNotRunning : Exception
    {
        public MicrosoftDefenderNotRunning() : base("Microsoft Defender is not running")
        {
        }
    }

    internal class NetworkAdapterNotEnergySavingException : Exception
    {
        public NetworkAdapterNotEnergySavingException() : base("Your network card(s) does not support power saving mode")
        {
        }
    }

    internal class NoInternetConnectionException : Exception
    {
        public NoInternetConnectionException() : base("No Internet connection")
        {
        }
    }

    internal class OneDriveIsInstalledException : Exception
    {
        public OneDriveIsInstalledException() : base("OneDrive is installed on this PC")
        {
        }
    }

    internal class OneDriveNotInstalledException : Exception
    {
        public OneDriveNotInstalledException() : base("OneDrive is not installed on this PC")
        {
        }
    }

    internal class PcIsVirtualMachineException : Exception
    {
        public PcIsVirtualMachineException() : base("The computer should not be a virtual machine")
        {
        }
    }

    internal class PcJoinedToDomainException : Exception
    {
        public PcJoinedToDomainException() : base("PC is domain-joined")
        {
        }
    }

    internal class PcNotJoinedToDomainException : Exception
    {
        public PcNotJoinedToDomainException() : base("PC is not domain-joined")
        {
        }
    }

    internal class RegistryKeyNotFoundException : Exception
    {
        public RegistryKeyNotFoundException(string key) : base($"The registry key \"{key}\" wasn't found")
        {
        }
    }

    internal class RegistryKeyUnexpectedValue : Exception
    {
        public RegistryKeyUnexpectedValue(string key) : base($"The registry key \"{key}\" has unexpected value")
        {
        }
    }

    internal class SheduledTaskNotFoundException : Exception
    {
        public SheduledTaskNotFoundException(string name) : base($"Scheduled task {name} wasn't found in OS")
        {
        }
    }

    internal class UpdateNotInstalledException : Exception
    {
        public UpdateNotInstalledException(string kbID) : base($"Update {kbID} is not installed on this PC")
        {
        }
    }

    internal class UwpAppFoundException : Exception
    {
        public UwpAppFoundException(string name) : base($"The UWP package {name} found in OS")
        {
        }
    }

    internal class UwpAppNotFoundException : Exception
    {
        public UwpAppNotFoundException(string name) : base($"The UWP package {name} wasn't found in OS")
        {
        }
    }

    internal class UwpNotSupportedVersion : Exception
    {
        public UwpNotSupportedVersion(string packageFullName) : base($"This UWP package version {packageFullName} not supported")
        {
        }
    }

    internal class VisualRedistrLibsLastVersionException : Exception
    {
        public VisualRedistrLibsLastVersionException() : base("The latest version of Visual C++ Redistributable 2015–2022 is installed")
        {
        }
    }

    internal class VisualRedistrLibsNotInstalled : Exception
    {
        public VisualRedistrLibsNotInstalled() : base("The Visual C++ Redistributable 2015–2022 is not installed on this PC")
        {
        }
    }

    internal class VitualizationNotSupportedException : Exception
    {
        public VitualizationNotSupportedException() : base("The virtualization (VT-x/SVM) isn't enabled in UEFI (BIOS)")
        {
        }
    }

    internal class WddmMinimalVersionException : Exception
    {
        public WddmMinimalVersionException(string minimumVersion, string currentVersion) : base($"WDDM minimum version must be: {minimumVersion}, current WDDM version: {currentVersion}")
        {
        }
    }

    internal class WindowsBuildNotSupportedException : Exception
    {
        public WindowsBuildNotSupportedException() : base("This OS build is not supported")
        {
        }
    }

    internal class WindowsCapabilityNotInstalledException : Exception
    {
        public WindowsCapabilityNotInstalledException(string name) : base($"The Windows capability {name} wasn't installed in this OS")
        {
        }
    }

    internal class WindowsEditionNotSupportedException : Exception
    {
        public WindowsEditionNotSupportedException() : base("Unsupported Windows edition")
        {
        }
    }

    internal class WrongApplicationVersionException : Exception
    {
        public WrongApplicationVersionException(string name, Version current, Version expected) : base($"The {name} version is {current}, minimal supported version {expected}")
        {
        }
    }

    internal class WrongGeoIdException : Exception
    {
        public WrongGeoIdException(int expectedGeoId, int currentGeoId) : base($"Wrong GeoId. Expected value {expectedGeoId}, received {currentGeoId}")
        {
        }
    }
}