using SophiApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    public static class ExtensionsHelper
    {
        public static void AddDataObject(this List<CustomActionDto> list, uint id, Action<bool> action, bool parameter) => list.Add(new CustomActionDto()
        {
            Id = id,
            Action = action,
            Parameter = parameter
        });

        public static bool ContainsId(this List<CustomActionDto> list, uint id) => !(list.FirstOrDefault(action => action.Id == id) is null);

        public static bool HasNullOrValue(this int? integer, int value) => integer is null || integer == value;

        public static bool HasNullOrValue(this string str, string value) => str is null || str == value;

        public static bool HasValue(this int? integer, int value) => integer == value;

        public static bool Invert(this bool value) => !value;

        public static List<T> Merge<T>(this List<T> source, List<T> mergeable)
        {
            source.AddRange(mergeable);
            return source;
        }

        public static void RemoveDataObject(this List<CustomActionDto> list, uint id) => list.Remove(list.Find(action => action.Id == id));

        public static List<string> Split(this List<string> source, string splitter)
        {
            source.Add(splitter);
            return source;
        }

        public static ushort ToUshort(this object value) => Convert.ToUInt16(value);
    }
}