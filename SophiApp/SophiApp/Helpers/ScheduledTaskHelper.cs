using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class ScheduledTaskHelper
    {
        private static Task GetTask(string taskPath, string taskName) => TaskService.Instance.GetTask($@"{taskPath}\{taskName}");

        internal static void Delete(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                TaskService.Instance.RootFolder.DeleteTask(task.Name);
        }

        internal static IEnumerable<Task> FindAll(Predicate<Task> filter) => TaskService.Instance.FindAllTasks(filter);

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

        internal static void TryChangeTaskState(string taskPath, string taskName, bool enable)
        {
            var task = GetTask(taskPath, taskName);
            if (task != null)
                task.Enabled = enable;
        }
    }
}