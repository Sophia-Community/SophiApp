using SophiAppCE.Actions.Privacy;
using SophiAppCE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    internal static class ControlModelActions
    {
        internal static IAction SetAction(string tag, ushort id)
        {
            // HACK - Delete Before Release !!!
            try
            {                
                return Activator.CreateInstance(Type.GetType($"SophiAppCE.Actions.{tag}._{id}")) as IAction;
            }
            catch (Exception)
            {
                // HACK - Delete Before Release !!!
                return new _100();
            }
        }
    }
}
