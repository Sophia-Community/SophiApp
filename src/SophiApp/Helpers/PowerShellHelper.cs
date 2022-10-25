using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace SophiApp.Helpers
{
    internal class PowerShellHelper
    {
        private const string getUwpUpdate = @"Get-CimInstance -Namespace 'Root\cimv2\mdm\dmmap' -ClassName 'MDM_EnterpriseModernAppManagement_AppManagement01' | Invoke-CimMethod -MethodName UpdateScanMethod";

        internal static T GetScriptProperty<T>(string script, string propertyName) => (T)InvokeScript(script).First().Properties[propertyName].Value;

        internal static T GetScriptResult<T>(string script) => (T)InvokeScript(script).First().BaseObject;

        internal static void GetUwpAppsUpdates() => InvokeScript(getUwpUpdate);

        internal static Collection<PSObject> InvokeScript(string script) => PowerShell.Create().AddScript(script).Invoke();
    }
}