using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class ComObjectHelper
    {
        internal static dynamic CreateFromProgID(string progID)
        {
            var type = Type.GetTypeFromProgID(progID);
            return Activator.CreateInstance(type);
        }

        internal static IEnumerable<string> GetOpenedFolders()
        {
            const string comShellApp = "Shell.Application";

            foreach (var folder in ComObjectHelper.CreateFromProgID(comShellApp).Windows())
            {
                yield return folder.Document.Folder.Self.Path;
            }
        }
    }
}