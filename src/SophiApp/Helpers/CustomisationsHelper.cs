using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class CustomisationsHelper
    {
        private const string CUSTOMISATION_OS_CLASS = "SophiApp.Customisations.CustomisationOs";
        private const string CUSTOMISATION_STATUS_CLASS = "SophiApp.Customisations.CustomisationStatus";
        private const string MAGIC_METHOD = "ItsMagic";

        internal static Action<bool> GetCustomisationOs(uint id)
        {
            var type = Type.GetType(CUSTOMISATION_OS_CLASS);
            var method = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(typeof(Action<bool>), method) as Action<bool>;
        }

        internal static Func<bool> GetCustomisationStatus(uint id)
        {
            var type = Type.GetType(CUSTOMISATION_STATUS_CLASS);
            var method = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public) ?? type.GetMethod(MAGIC_METHOD, BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(typeof(Func<bool>), method) as Func<bool>;
        }
    }
}