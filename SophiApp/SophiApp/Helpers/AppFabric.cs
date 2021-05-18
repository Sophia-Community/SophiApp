using SophiApp.Commons;
using SophiApp.Models;
using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class AppFabric
    {
        private const string CURRENT_STATE_ACTION_CLASS = "SophiApp.Actions.CurrentStateAction";
        private const string SYSTEM_STATE_ACTION_CLASS = "SophiApp.Actions.SystemStateAction";

        private static Func<bool> FindCurrentStateAction(uint id)
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

        private static Action FindSystemStateAction(uint id)
        {
            try
            {
                var type = Type.GetType(SYSTEM_STATE_ACTION_CLASS);
                var action = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Action), action) as Action;
            }
            catch (Exception e)
            {
                //TODO: FOR DEBUG ONLY !!!
                var type = Type.GetType(SYSTEM_STATE_ACTION_CLASS);
                var action = type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(typeof(Action), action) as Action;
            }
        }

        internal static ExpandingGroup CreateExpandingGroupModel(JsonDTO json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            return Activator.CreateInstance(model, json) as ExpandingGroup;
        }

        internal static RadioButtonGroup CreateRadioButtonGroupModel(JsonDTO json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            return Activator.CreateInstance(model, json) as RadioButtonGroup;
        }

        internal static BaseTextedElement CreateTextElementModel(JsonDTO json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            var element = Activator.CreateInstance(model, json) as BaseTextedElement;
            element.CurrentStateAction = FindCurrentStateAction(element.Id);
            element.SystemStateAction = FindSystemStateAction(element.Id);
            return element;
        }
    }
}