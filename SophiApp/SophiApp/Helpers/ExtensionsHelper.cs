using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    public static class ExtensionsHelper
    {
        public static void AddDataObject(this List<CustomActionDTO> list, uint id, Action<bool> action, bool parameter) => list.Add(new CustomActionDTO()
        {
            Id = id,
            Action = action,
            Parameter = parameter
        });

        public static bool ContainsId(this List<CustomActionDTO> list, uint id) => !(list.FirstOrDefault(action => action.Id == id) is null);

        public static bool HasNullOrValue(this int? integer, int value) => integer is null || integer == value;

        public static bool Invert(this bool value) => !value;

        public static void RemoveDataObject(this List<CustomActionDTO> list, uint id) => list.Remove(list.Find(action => action.Id == id));
    }
}