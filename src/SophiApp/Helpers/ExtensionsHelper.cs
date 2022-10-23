using SophiApp.Customisations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    public static class ExtensionsHelper
    {
        internal static void AddAction(this List<Customisation> list, uint id, Action<bool> action, bool parameter) => list.Add(new Customisation(id, action, parameter));

        internal static void AddAction(this List<Customisation> list, string packageFullName, Action<string, bool> action, bool forAllUsers) => list.Add(new UwpCustomisation(packageFullName, action, forAllUsers));

        internal static bool ContainsId(this List<Customisation> list, uint id) => !(list.FirstOrDefault(action => action.Id == id) is null);

        internal static bool ContainsId(this List<Customisation> list, string id) => (list.Where(customisation => customisation is UwpCustomisation)
                                                                                          .Cast<UwpCustomisation>()
                                                                                          .FirstOrDefault(customisation => customisation.Id == id) is null)
                                                                                          .Invert();

        internal static void RemoveAction(this List<Customisation> list, uint id) => list.Remove(list.Find(action => action.Id == id));

        internal static void RemoveAction(this List<Customisation> list, string id) => list.Remove(list.Where(customisation => customisation is UwpCustomisation)
                                                                                                       .Cast<UwpCustomisation>()
                                                                                                       .FirstOrDefault(customisation => customisation.Id == id));

        public static T GetFirstValue<T>(this object obj) => (T)Convert.ChangeType(obj is object[]? (obj as object[]).First() : obj, typeof(T));

        public static bool HasNullOrValue(this int? integer, int value) => integer is null || integer == value;

        public static bool HasNullOrValue(this string str, string value) => str is null || str == value;

        public static bool HasValue(this int? integer, int value) => integer == value;

        public static bool Invert(this bool value) => !value;

        public static List<T> Merge<T>(this List<T> source, List<T> mergeable)
        {
            if (mergeable.NotEmpty())
                source.AddRange(mergeable);

            return source;
        }

        public static bool NotEmpty<T>(this List<T> list) => list.Count > 0;

        public static List<string> Split(this List<string> source, string splitter)
        {
            if (source.NotEmpty() && source.Last() != string.Empty)
                source.Add(splitter);

            return source;
        }

        public static int ToInt32(this object value) => Convert.ToInt32(value);

        public static ushort ToUshort(this object value) => Convert.ToUInt16(value);
    }
}