using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace SophiApp.Helpers
{
    internal class PowerShellHelper
    {
        internal static T GetScriptProperty<T>(string script, string propertyName) => (T)InvokeScript(script).FirstOrDefault().Properties[propertyName].Value;

        internal static T GetScriptResult<T>(string script) => (T)InvokeScript(script).FirstOrDefault().BaseObject;

        internal static Collection<PSObject> InvokeScript(string script) => PowerShell.Create().AddScript(script).Invoke();
    }
}