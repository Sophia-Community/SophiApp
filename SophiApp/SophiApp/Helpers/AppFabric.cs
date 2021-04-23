using SophiApp.Commons;
using SophiApp.Models;
using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class AppFabric
    {
        private const string CURRENT_STATE_ACTION_CLASS = "SophiApp.Actions.CurrentStateAction";

        //TODO: Implement method selection by ID
        //private static Func<bool> GetCurrentStateAction(uint id)
        //{
        //    var type = Type.GetType(CURRENT_STATE_ACTION_CLASS);
        //    var action = type.GetMethod($"{id}", BindingFlags.Static | BindingFlags.Public);
        //    return Delegate.CreateDelegate(typeof(Func<bool>), action) as Func<bool>;
        //}

        //TODO: FOR DEBUG ONLY !!!
        private static Func<bool> GetCurrentStateAction(uint id)
        {
            var type = Type.GetType(CURRENT_STATE_ACTION_CLASS);
            var action = type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(typeof(Func<bool>), action) as Func<bool>;
        }

        internal static BaseContainer CreateContainerModel(JsonDTO json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            return Activator.CreateInstance(model, json) as BaseContainer;
        }

        internal static BaseTextedElement CreateTextElementModel(JsonDTO json)
        {
            var model = Type.GetType($"SophiApp.Models.{json.Model}");
            var element = Activator.CreateInstance(model, json) as BaseTextedElement;
            element.CurrentStateAction = GetCurrentStateAction(element.Id);
            return element;
        }
    }
}