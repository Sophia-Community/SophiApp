using System;

namespace SophiApp.Helpers
{
    internal class ComObjectHelper
    {
        internal static dynamic CreateFromProgID(string progID)
        {
            var type = Type.GetTypeFromProgID(progID);
            return Activator.CreateInstance(type);
        }
    }
}