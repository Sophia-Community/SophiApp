using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Commons
{
    class Fabric
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
