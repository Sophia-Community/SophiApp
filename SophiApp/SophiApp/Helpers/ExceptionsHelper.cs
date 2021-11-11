using System;

namespace SophiApp.Helpers
{
    internal class BitlockerIsEnabledException : Exception
    {
        public BitlockerIsEnabledException() : base("BitLocker protection is enabled")
        {
        }
    }

    internal class NetworkAdapterNotEnergySavingException : Exception
    {
        public NetworkAdapterNotEnergySavingException() : base("Your network card(s) does not support power saving mode")
        {
        }
    }

    internal class PcJoinedToDomainException : Exception
    {
        public PcJoinedToDomainException() : base("PC is joined to domain")
        {
        }
    }

    internal class PcNotJoinedToDomainException : Exception
    {
        public PcNotJoinedToDomainException() : base("PC is not joined to domain")
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
        public SheduledTaskNotFoundException(string name) : base($"Scheduled task {name} wasn't found on OS")
        {
        }
    }

    internal class UwpAppNotFoundException : Exception
    {
        public UwpAppNotFoundException(string name) : base($"The UWP package {name} wasn't found on OS")
        {
        }
    }

    internal class WindowsCapabilityNotInstalledException : Exception
    {
        public WindowsCapabilityNotInstalledException(string name) : base($"The Windows capability {name} wasn't installed on this OS")
        {
        }
    }

    internal class WindowsEditionNotSupportedException : Exception
    {
        public WindowsEditionNotSupportedException() : base("Unsupported Windows edition")
        {
        }
    }
}