using SophiApp.Interfaces;
using System;

namespace SophiApp.Commons
{
    internal class Fabric
    {
        internal static IUIElementModel CreateElementModel(JsonDTO json, UILanguage language)
        {
            var type = Type.GetType($"SophiApp.Models.{json.Type}");
            var element = Activator.CreateInstance(type, json) as IUIElementModel;
            element.SetLocalizationTo(language);
            return element;
        }
    }
}