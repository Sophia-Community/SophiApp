using SophiApp.Commons;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class ElementsFabric
    {
        private const string CURRENT_STATE_ACTION_CLASS = "SophiApp.Actions.CurrentStateAction";
        private const string SYSTEM_STATE_ACTION_CLASS = "SophiApp.Actions.SystemStateAction";

        internal static BaseTextedElement Create(JsonDTO json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            var element = Activator.CreateInstance(model, json) as BaseTextedElement;            
            element.CurrentStateAction = GetCurrentStateAction(element.Id);
            element.SystemStateAction = GetSystemStateAction(element.Id);
            return element;
        }

        private static Action<bool> GetSystemStateAction(uint id)
        {
            try
            {
                var type = Type.GetType(SYSTEM_STATE_ACTION_CLASS);
                var action = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Action), action) as Action<bool>;
            }
            catch (Exception e)
            {
                //TODO: FOR DEBUG ONLY !!!
                var type = Type.GetType(SYSTEM_STATE_ACTION_CLASS);
                var action = type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Action<bool>), action) as Action<bool>;
            }
        }

        private static Func<bool> GetCurrentStateAction(uint id)
        {
            try
            {
                var type = Type.GetType(CURRENT_STATE_ACTION_CLASS);
                var action = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Func<bool>), action) as Func<bool>;
            }
            catch (Exception e)
            {
                //TODO: FOR DEBUG ONLY !!!
                var type = Type.GetType(CURRENT_STATE_ACTION_CLASS);
                var action = type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Func<bool>), action) as Func<bool>;
            }
        }
    }
}
