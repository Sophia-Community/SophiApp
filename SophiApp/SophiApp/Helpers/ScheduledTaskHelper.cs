using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class ScheduledTaskHelper
    {
        internal static void DeleteTask(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                TaskService.Instance.RootFolder.DeleteTask(task.Name);
        }

        internal static void DeleteTask(string task, bool throwNotExist) => TaskService.Instance.RootFolder.DeleteTask(task, throwNotExist);

        internal static void DeleteTask(IEnumerable<string> tasks, bool throwNotExist)
        {
            foreach (var task in tasks)
                DeleteTask(task, throwNotExist);
        }

        internal static bool Exist(string taskPath, string taskName) => (GetTask(taskPath, taskName) is null).Invert();

        internal static IEnumerable<Task> FindAll(Predicate<Task> filter) => TaskService.Instance.FindAllTasks(filter);

        internal static TaskState GetTaskState(string taskPath, string taskName) => TaskService.Instance.GetTask($@"{taskPath}\{taskName}")?.State ?? throw new SheduledTaskNotFoundException(taskName);

        internal static void RegisterLogonTask(string name, string description, string execute, string arg, string username)
        {
            var td = TaskService.Instance.NewTask();
            td.Triggers.Add(new LogonTrigger() { UserId = username });
            td.Actions.Add(execute, arg);
            td.Principal.UserId = username;
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Settings.Compatibility = TaskCompatibility.V2_2;
            td.RegistrationInfo.Author = AppHelper.AppName;
            td.RegistrationInfo.Description = description;
            _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(name, td);
        }

        internal static void RegisterTask(string taskName, string taskDescription, string execute, string arg, string userName, TaskRunLevel runLevel, Trigger trigger)
        {
            var td = TaskService.Instance.NewTask();
            td.Triggers.Add(trigger);
            td.Actions.Add(execute, arg);
            td.Principal.UserId = userName;
            td.Principal.RunLevel = runLevel;
            td.Settings.Compatibility = TaskCompatibility.V2_2;
            td.Settings.StartWhenAvailable = true;
            td.RegistrationInfo.Author = AppHelper.AppName;
            td.RegistrationInfo.Description = taskDescription;
            _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
        }

        internal static void TryChangeTaskState(string taskPath, string taskName, bool enable)
        {
            var task = GetTask(taskPath, taskName);
            if (task != null)
                task.Enabled = enable;
        }

        private static Task GetTask(string taskPath, string taskName) => TaskService.Instance.GetTask($@"{taskPath}\{taskName}");
    }
}