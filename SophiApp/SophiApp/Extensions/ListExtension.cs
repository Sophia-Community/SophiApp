using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Extensions
{
    public static class ListExtension
    {
        public static void AddDataObject(this List<CustomActionDTO> list, uint id, Action<bool> action, bool parameter) => list.Add(new CustomActionDTO()
        {
            Id = id,
            Action = action,
            Parameter = parameter
        });

        public static bool ContainsId(this List<CustomActionDTO> list, uint id) => !(list.FirstOrDefault(action => action.Id == id) is null);

        public static void RemoveDataObject(this List<CustomActionDTO> list, uint id) => list.Remove(list.Find(action => action.Id == id));
    }
}