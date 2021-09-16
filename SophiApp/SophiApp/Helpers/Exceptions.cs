using System;
using System.Runtime.Serialization;

namespace SophiApp.Helpers
{
    internal class BitlockerIsEnabledException : Exception
    {
        protected BitlockerIsEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BitlockerIsEnabledException() : base("BitLocker protection is enabled")
        {
        }

        public BitlockerIsEnabledException(Exception inner) : base("BitLocker protection is enabled", inner)
        {
        }
    }

    internal class UwpAppNotFoundException : Exception
    {
        protected UwpAppNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UwpAppNotFoundException(string name) : base($"The UWP package {name} wasn't found on OS")
        {
        }

        public UwpAppNotFoundException(string name, Exception inner) : base($"The UWP package {name} wasn't found on OS", inner)
        {
        }
    }

    internal class WindowsCapabilityNotInstalledException : Exception
    {
        protected WindowsCapabilityNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public WindowsCapabilityNotInstalledException(string name) : base($"The Windows capability {name} wasn't installed on this OS")
        {
        }

        public WindowsCapabilityNotInstalledException(string name, Exception inner) : base($"The Windows capability {name} wasn't installed on this OS", inner)
        {
        }
    }

    internal class WindowsEditionNotSupportedException : Exception
    {
        protected WindowsEditionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public WindowsEditionNotSupportedException() : base("Unsupported Windows edition")
        {
        }

        public WindowsEditionNotSupportedException(Exception inner) : base("Unsupported Windows edition", inner)
        {
        }
    }
}