using System;

namespace SophiApp.Helpers
{
    public class AdapterTypeInternalOrNullException : Exception
    {
        public AdapterTypeInternalOrNullException(string dacType) : base($"Video adapter DAC type should not be a internal or null")
        {
        }
    }

    public class BitlockerIsEnabledException : Exception
    {
        public BitlockerIsEnabledException() : base("BitLocker protection is enabled")
        {
        }
    }

    public class DotNetInstalledException : Exception
    {
        public DotNetInstalledException(Version version) : base($".Net version {version} already installed on this PC")
        {
        }
    }

    public class DotNetNotInstalledException : Exception
    {
        public DotNetNotInstalledException(Version version) : base($".Net version {version} is not installed on this PC")
        {
        }
    }

    public class FileNotExistException : Exception
    {
        public FileNotExistException(string filePath) : base($"File does not exist \"{filePath}\"")
        {
        }
    }

    public class MicrosoftDefenderDisabledByGroupPolicy : Exception
    {
        public MicrosoftDefenderDisabledByGroupPolicy() : base("Microsoft Defender disabled by group policy")
        {
        }
    }

    public class MicrosoftDefenderNotRunning : Exception
    {
        public MicrosoftDefenderNotRunning() : base("Microsoft Defender is not running")
        {
        }
    }

    public class NetworkAdapterNotEnergySavingException : Exception
    {
        public NetworkAdapterNotEnergySavingException() : base("Your network card(s) does not support power saving mode")
        {
        }
    }

    public class NoInternetConnectionException : Exception
    {
        public NoInternetConnectionException() : base("No Internet connection")
        {
        }
    }

    public class OneDriveIsInstalledException : Exception
    {
        public OneDriveIsInstalledException() : base("OneDrive is installed on this PC")
        {
        }
    }

    public class OneDriveNotInstalledException : Exception
    {
        public OneDriveNotInstalledException() : base("OneDrive is not installed on this PC")
        {
        }
    }

    public class PcIsVirtualMachineException : Exception
    {
        public PcIsVirtualMachineException() : base("The computer should not be a virtual machine")
        {
        }
    }

    public class PcJoinedToDomainException : Exception
    {
        public PcJoinedToDomainException() : base("PC is domain-joined")
        {
        }
    }

    public class PcNotJoinedToDomainException : Exception
    {
        public PcNotJoinedToDomainException() : base("PC is not domain-joined")
        {
        }
    }

    public class RegistryKeyNotFoundException : Exception
    {
        public RegistryKeyNotFoundException(string key) : base($"The registry key \"{key}\" wasn't found")
        {
        }
    }

    public class RegistryKeyUnexpectedValue : Exception
    {
        public RegistryKeyUnexpectedValue(string key) : base($"The registry key \"{key}\" has unexpected value")
        {
        }
    }

    public class SheduledTaskNotFoundException : Exception
    {
        public SheduledTaskNotFoundException(string name) : base($"Scheduled task {name} wasn't found in OS")
        {
        }
    }

    public class UpdateNotInstalledException : Exception
    {
        public UpdateNotInstalledException(string kbID) : base($"Update {kbID} is not installed on this PC")
        {
        }
    }

    public class UwpAppFoundException : Exception
    {
        public UwpAppFoundException(string name) : base($"The UWP package {name} found in OS")
        {
        }
    }

    public class UwpAppNotFoundException : Exception
    {
        public UwpAppNotFoundException(string name) : base($"The UWP package {name} wasn't found in OS")
        {
        }
    }

    public class UwpNotSupportedVersion : Exception
    {
        public UwpNotSupportedVersion(string packageFullName) : base($"This UWP package version {packageFullName} not supported")
        {
        }
    }

    public class VisualRedistrLibsLastVersionException : Exception
    {
        public VisualRedistrLibsLastVersionException() : base("The latest version of Visual C++ Redistributable 2015–2022 x64 is installed")
        {
        }
    }

    public class VisualRedistrLibsNotInstalled : Exception
    {
        public VisualRedistrLibsNotInstalled() : base("The Visual C++ Redistributable 2015–2022 x64 is not installed on this PC")
        {
        }
    }

    public class VitualizationNotSupportedException : Exception
    {
        public VitualizationNotSupportedException() : base("The virtualization (VT-x/SVM) isn't enabled in UEFI (BIOS)")
        {
        }
    }

    public class WddmMinimalVersionException : Exception
    {
        public WddmMinimalVersionException(string minimumVersion, string currentVersion) : base($"WDDM minimum version must be: {minimumVersion}, current WDDM version: {currentVersion}")
        {
        }
    }

    public class WindowsBuildNotSupportedException : Exception
    {
        public WindowsBuildNotSupportedException() : base("This OS build is not supported")
        {
        }
    }

    public class WindowsCapabilityNotInstalledException : Exception
    {
        public WindowsCapabilityNotInstalledException(string name) : base($"The Windows capability {name} wasn't installed in this OS")
        {
        }
    }

    public class WindowsEditionNotSupportedException : Exception
    {
        public WindowsEditionNotSupportedException() : base("Unsupported Windows edition")
        {
        }
    }

    public class WrongGeoIdException : Exception
    {
        public WrongGeoIdException(int expectedGeoId, int currentGeoId) : base($"Wrong GeoId. Expected value {expectedGeoId}, received {currentGeoId}")
        {
        }
    }
}