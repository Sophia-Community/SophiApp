using SophiApp.Commons;
using SophiApp.Models;
using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    //TODO: ElementsFabric - deprecated !!!
    internal class ElementsFabric
    {
        //TODO: ElementsFabric - deprecated !!!
        private const string CURRENT_STATE_ACTION_CLASS = "SophiApp.Actions.CurrentStateAction";

        private const string SYSTEM_STATE_ACTION_CLASS = "SophiApp.Actions.SystemStateAction";

        //TODO: ElementsFabric - BaseTextedElement Deprecated !!!
        internal static BaseTextedElement Create(JsonDto json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            var element = Activator.CreateInstance(model, json) as BaseTextedElement;
            return element;
        }

        internal static TextedElement CreateTextedElement(JsonGuiDto dto)
        {
            var type = Type.GetType($"SophiApp.Models.{dto.Type}");
            return Activator.CreateInstance(type, dto) as TextedElement;
        }

        //TODO: SetCurreElementsFabricntStateAction - SetCurrentStateAction Deprecated !!!
        internal static Func<bool> SetCurrentStateAction(uint id)
        {
            try
            {
                var type = Type.GetType(CURRENT_STATE_ACTION_CLASS);
                var action = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Func<bool>), action) as Func<bool>;
            }
            catch (Exception e)
            {
                //TODO: SetCurrentStateAction FOR DEBUG ONLY !!!
                var type = Type.GetType(CURRENT_STATE_ACTION_CLASS);
                var action = type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Func<bool>), action) as Func<bool>;
            }
        }

        //TODO: ElementsFabric - SetSystemStateAction Deprecated !!!
        internal static Action<bool> SetSystemStateAction(uint id)
        {
            try
            {
                var type = Type.GetType(SYSTEM_STATE_ACTION_CLASS);
                var action = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Action<bool>), action) as Action<bool>;
            }
            catch (Exception)
            {
                //TODO: SetSystemStateAction FOR DEBUG ONLY !!!
                var type = Type.GetType(SYSTEM_STATE_ACTION_CLASS);
                var action = type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Action<bool>), action) as Action<bool>;
            }
        }
    }
}