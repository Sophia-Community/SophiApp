using SophiApp.Interfaces;
using System;

namespace SophiApp.Commons
{
    internal class Fabric
    {
        internal static IUIElementModel CreateElementModel(JsonDTO json)
        {
            var type = Type.GetType($"SophiApp.Models.{json.Type}");
            return Activator.CreateInstance(type, json) as IUIElementModel;
        }
    }
}