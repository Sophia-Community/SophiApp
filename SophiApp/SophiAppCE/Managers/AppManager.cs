using SophiAppCE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Managers
{
    internal class AppManager
    {
        internal string AppBaseDir { get; } = AppDomain.CurrentDomain.BaseDirectory;
        internal IEnumerable<JsonObject> ScriptsData { get; private set; } = GuiManager.GetSettingsJson();
        public AppManager()
        {

        }
    }
}
