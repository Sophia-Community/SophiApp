using SophiApp.Commons;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    class AppFabric
    {
        internal static BaseElement CreateElementModel(JsonDTO json)
        {
            var modelType = Type.GetType($"SophiApp.Models.{json.Model}");
            var a = Activator.CreateInstance(modelType, json) as BaseElement;
            return a;
        }
    }
}
