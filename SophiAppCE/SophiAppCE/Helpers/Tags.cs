using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Helpers
{
    public struct Tags
    {        
        public static readonly string Privacy = Application.Current.TryFindResource("Tag.Privacy") as string;
        public static readonly string Ui = Application.Current.TryFindResource("Tag.Ui") as string;
        public static readonly string ContextMenu = Application.Current.TryFindResource("Tag.ContextMenu") as string;
        public static readonly string StartMenu = Application.Current.TryFindResource("Tag.StartMenu") as string;
        public static readonly string System = Application.Current.TryFindResource("Tag.System") as string;
        public static readonly string TaskSheduler = Application.Current.TryFindResource("Tag.TaskSheduler") as string;
        public static readonly string Security = Application.Current.TryFindResource("Tag.Security") as string;
        public static readonly string Game = Application.Current.TryFindResource("Tag.Game") as string;
        public static readonly string Uwp = Application.Current.TryFindResource("Tag.Uwp") as string;
        public static readonly string OneDrive = Application.Current.TryFindResource("Tag.OneDrive") as string;
        public static readonly string ApplyAll = Application.Current.TryFindResource("Tag.ApplyAll") as string;
        public static readonly string Settings = Application.Current.TryFindResource("Tag.Settings") as string;
    }
}
