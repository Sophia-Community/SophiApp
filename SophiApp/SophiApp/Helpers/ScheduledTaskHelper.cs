using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SophiApp.Helpers
{
    internal class ScheduledTaskHelper
    {
        internal static Task GetTask(string name) => TaskService.Instance.RootFolder.EnumerateTasks(task => task.Name.Equals(name), true).FirstOrDefault();

        internal static void EnableTask(string name) => GetTask(name).Enabled = true;

        internal static void DisableTask(string name) => GetTask(name).Enabled = false;

    }
}
