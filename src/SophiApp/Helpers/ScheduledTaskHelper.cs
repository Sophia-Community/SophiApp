using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class ScheduledTaskHelper
    {
        private static Task GetTask(string taskPath, string taskName) => TaskService.Instance.GetTask($@"{taskPath}\{taskName}");

        internal static void DeleteTask(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                TaskService.Instance.RootFolder.DeleteTask(task.Name);
        }

        internal static void DeleteTask(string task, bool throwNotExist) => TaskService.Instance.RootFolder.DeleteTask(task, throwNotExist);

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

        internal static void RegisterTask(string taskName, string taskDescription, string execute, string arg, string userName, TaskRunLevel runLevel)
        {
            var task = TaskService.Instance.NewTask();
            task.Actions.Add(execute, arg);
            task.Principal.UserId = userName;
            task.Principal.RunLevel = runLevel;
            task.Settings.Compatibility = TaskCompatibility.V2_2;
            task.Settings.StartWhenAvailable = true;
            task.RegistrationInfo.Author = AppHelper.AppName;
            task.RegistrationInfo.Description = taskDescription;
            _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, task);
        }

        internal static void RegisterTask(string taskName, string taskDescription, string execute, string arg, string userName, TaskRunLevel runLevel, Trigger trigger)
        {
            var task = TaskService.Instance.NewTask();
            task.Triggers.Add(trigger);
            task.Actions.Add(execute, arg);
            task.Principal.UserId = userName;
            task.Principal.RunLevel = runLevel;
            task.Settings.Compatibility = TaskCompatibility.V2_2;
            task.Settings.StartWhenAvailable = true;
            task.RegistrationInfo.Author = AppHelper.AppName;
            task.RegistrationInfo.Description = taskDescription;
            _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, task);
        }

        internal static void TryChangeTaskState(string taskPath, string taskName, bool enable)
        {
            var task = GetTask(taskPath, taskName);
            if (task != null)
                task.Enabled = enable;
        }

        internal static void TryDeleteFolder(params string[] folders)
        {
            foreach (var folder in folders)
            {
                try
                {
                    TryDeleteTask(FindAll(task => task.Folder.Name == folder));
                    TryDeleteFolder(folder);
                }
                catch (Exception)
                {
                }
            }
        }

        internal static void TryDeleteFolder(string folder)
        {
            try
            {
                TaskService.Instance.RootFolder.DeleteFolder(folder, false);
            }
            catch (Exception)
            {
            }
        }

        internal static void TryDeleteTask(params string[] tasks)
        {
            foreach (var task in tasks)
            {
                try
                {
                    DeleteTask(task, false);
                }
                catch (Exception)
                {
                }
            }
        }

        internal static void TryDeleteTask(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
            {
                try
                {
                    TaskService.Instance.RootFolder.DeleteTask(task.Path, false);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}