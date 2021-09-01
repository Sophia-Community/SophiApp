using System;

namespace SophiApp.Helpers
{
    internal class BitlockerEnabledException : Exception
    {
        protected BitlockerEnabledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public BitlockerEnabledException() : base("BitLocker protection is enabled")
        {
        }

        public BitlockerEnabledException(Exception inner) : base("BitLocker protection is enabled", inner)
        {
        }
    }

    [global::System.Serializable]
    internal class UwpAppNotFoundException : Exception
    {
        protected UwpAppNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public UwpAppNotFoundException(string name) : base($"UWP app {name} not found in this PC")
        {
        }

        public UwpAppNotFoundException(string name, Exception inner) : base($"UWP app {name} not found in this PC", inner)
        {
        }
    }

    internal class WindowsCapabilityNotInstalledException : Exception
    {
        protected WindowsCapabilityNotInstalledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public WindowsCapabilityNotInstalledException(string name) : base($"Windows capability {name} not installed in this PC")
        {
        }

        public WindowsCapabilityNotInstalledException(string name, Exception inner) : base($"Windows capability {name} not installed in this PC", inner)
        {
        }
    }

    internal class WindowsEditionNotSupportedException : Exception
    {
        protected WindowsEditionNotSupportedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public WindowsEditionNotSupportedException() : base("Unsupported Windows edition")
        {
        }

        public WindowsEditionNotSupportedException(Exception inner) : base("Unsupported Windows edition", inner)
        {
        }
    }
}