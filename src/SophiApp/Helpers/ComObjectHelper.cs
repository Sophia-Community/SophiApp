using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class ComObjectHelper
    {
        private const string MS_UPDATE_AUTO_UPDATE = "Microsoft.Update.AutoUpdate";
        private const string MS_UPDATE_SERVICE_MANAGER = "Microsoft.Update.ServiceManager";

        internal static dynamic CreateFromProgID(string progID)
        {
            var type = Type.GetTypeFromProgID(progID);
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Enable receiving updates for other Microsoft products when you update Windows.
        /// </summary>
        internal static void EnableUpdateForOtherProducts()
        {
            var updateManager = CreateFromProgID(MS_UPDATE_SERVICE_MANAGER);
            updateManager.AddService2("7971f918-a847-4430-9279-4a52d1efe18d", 7, "");
        }

        internal static IEnumerable<string> GetOpenedFolders()
        {
            const string comShellApp = "Shell.Application";

            foreach (var folder in ComObjectHelper.CreateFromProgID(comShellApp).Windows())
            {
                yield return folder.Document.Folder.Self.Path;
            }
        }

        internal static void SetWindowsUpdateDetectNow()
        {
            var updateManager = CreateFromProgID(MS_UPDATE_AUTO_UPDATE);
            updateManager.DetectNow();
        }
    }
}