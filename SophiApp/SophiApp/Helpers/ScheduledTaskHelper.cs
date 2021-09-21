using Microsoft.Win32.TaskScheduler;

namespace SophiApp.Helpers
{
    internal class ScheduledTaskHelper
    {
        private static Task GetTask(string taskPath, string taskName) => TaskService.Instance.GetTask($@"{taskPath}\{taskName}");

        internal static void ChangeTaskState(string taskPath, string taskName, bool enable) => GetTask(taskPath, taskName).Enabled = enable;

        internal static void DisableTask(string taskPath, string taskName) => GetTask(taskPath, taskName).Enabled = false;

        internal static void EnableTask(string taskPath, string taskName) => GetTask(taskPath, taskName).Enabled = true;

        internal static TaskState GetTaskState(string taskPath, string taskName) => TaskService.Instance.GetTask($@"{taskPath}\{taskName}")?.State ?? throw new SheduledTaskNotFoundException(taskName);
    }
}