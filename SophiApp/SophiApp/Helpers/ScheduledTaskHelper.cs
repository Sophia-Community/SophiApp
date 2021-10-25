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

        internal static void RegisterLogonTask(string name, string description, string execute, string args, string username)
        {
            var td = TaskService.Instance.NewTask();
            td.Triggers.Add(new LogonTrigger() { UserId = username });
            td.Actions.Add(execute, args);
            td.Principal.UserId = username;
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Settings.Compatibility = TaskCompatibility.V2_2;
            td.RegistrationInfo.Author = AppHelper.AppName;
            td.RegistrationInfo.Description = description;
            _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(name, td);
        }
    }
}