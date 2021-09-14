using Microsoft.Win32.TaskScheduler;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class ScheduledTaskHelper
    {
        internal static void DisableTask(string name) => GetTask(name).Enabled = false;

        internal static void EnableTask(string name) => GetTask(name).Enabled = true;

        internal static Task GetTask(string name) => TaskService.Instance.RootFolder.EnumerateTasks(task => task.Name.Equals(name), true).FirstOrDefault();
    }
}