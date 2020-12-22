using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SophiAppCE.Helpers
{
    internal static class ControlModelStates
    {
        internal static bool GetState(ushort Id)
        {
            // HACK - Delete Before Release !!!
            try
            {
                Type type = Type.GetType("SophiAppCE.Helpers.ControlModelStates");
                return (bool)type.InvokeMember(Convert.ToString($"_{Id}"), BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, null);
            }
            catch (Exception)
            {
                return false;                
            }            
        }

        public static bool _100() => WinService.GetStartupState("DiagTrack") == WinService.StartupType.Disabled ? true : false;      

    }
}
