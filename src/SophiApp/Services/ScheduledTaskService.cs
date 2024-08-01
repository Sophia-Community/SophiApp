// <copyright file="ScheduledTaskService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.Win32.TaskScheduler;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <inheritdoc/>
    public class ScheduledTaskService : IScheduledTaskService
    {
        private readonly TaskService taskScheduler;
        private readonly ICommonDataService commonDataService;
        private readonly string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);

        private readonly string cleanupTaskAction = @"-WindowStyle Hidden -Command Get-Process -Name cleanmgr, Dism, DismHost | Stop-Process -Force
$ProcessInfo = New-Object -TypeName System.Diagnostics.ProcessStartInfo
$ProcessInfo.FileName = """"""$env:SystemRoot\System32\cleanmgr.exe""""""
$ProcessInfo.Arguments = """"""/sagerun:1337""""""
$ProcessInfo.UseShellExecute = $true
$ProcessInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Minimized

$Process = New-Object -TypeName System.Diagnostics.Process
$Process.StartInfo = $ProcessInfo
$Process.Start() | Out-Null

Start-Sleep -Seconds 3

[int]$SourceMainWindowHandle = (Get-Process -Name cleanmgr | Where-Object -FilterScript {$_.PriorityClass -eq """"""BelowNormal""""""}).MainWindowHandle

while ($true)
{
	[int]$CurrentMainWindowHandle = (Get-Process -Name cleanmgr | Where-Object -FilterScript {$_.PriorityClass -eq """"""BelowNormal""""""}).MainWindowHandle
	if ($SourceMainWindowHandle -ne $CurrentMainWindowHandle)
	{
		$CompilerParameters = [System.CodeDom.Compiler.CompilerParameters]::new(""""""System.dll"""""")
		$CompilerParameters.TempFiles = [System.CodeDom.Compiler.TempFileCollection]::new($env:TEMP, $false)
		$CompilerParameters.GenerateInMemory = $true
		$Signature = @{
Namespace          = """"""WinAPI""""""
Name               = """"""Win32ShowWindowAsync""""""
Language           = """"""CSharp""""""
CompilerParameters = $CompilerParameters
MemberDefinition   = @""""""
[DllImport(""""""user32.dll"""""")]
public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
""""""@
		}

		if (-not (""""""WinAPI.Win32ShowWindowAsync"""""" -as [type]))
		{
            Add-Type @Signature
		}
		$MainWindowHandle = (Get-Process -Name cleanmgr | Where-Object -FilterScript {$_.PriorityClass -eq """"""BelowNormal""""""}).MainWindowHandle
		[WinAPI.Win32ShowWindowAsync]::ShowWindowAsync($MainWindowHandle, 2)

		break
	}

	Start-Sleep -Milliseconds 5
}

$ProcessInfo = New-Object -TypeName System.Diagnostics.ProcessStartInfo
$ProcessInfo.FileName = """"""$env:SystemRoot\System32\Dism.exe""""""
$ProcessInfo.Arguments = """"""/Online /English /Cleanup-Image /StartComponentCleanup /NoRestart""""""
$ProcessInfo.UseShellExecute = $true
$ProcessInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Minimized

$Process = New-Object -TypeName System.Diagnostics.Process
$Process.StartInfo = $ProcessInfo
$Process.Start() | Out-Null";

        private readonly string cleanupNotificationTaskAction = @"
# https://github.com/farag2/Sophia-Script-for-Windows
# https://t.me/sophia_chat

# Get Focus Assist status
# https://github.com/DCourtel/Windows_10_Focus_Assist/blob/master/FocusAssistLibrary/FocusAssistLib.cs
# https://redplait.blogspot.com/2018/07/wnf-ids-from-perfntcdll-adk-version.html

$CompilerParameters = [System.CodeDom.Compiler.CompilerParameters]::new(""""""System.dll"""""")
$CompilerParameters.TempFiles = [System.CodeDom.Compiler.TempFileCollection]::new($env:TEMP, $false)
$CompilerParameters.GenerateInMemory = $true
$Signature = @{
	Namespace          = ""WinAPI""
	Name               = ""Focus""
	Language           = ""CSharp""
	CompilerParameters = $CompilerParameters
	MemberDefinition   = @""
[DllImport(""NtDll.dll"", SetLastError = true)]
private static extern uint NtQueryWnfStateData(IntPtr pStateName, IntPtr pTypeId, IntPtr pExplicitScope, out uint nChangeStamp, out IntPtr pBuffer, ref uint nBufferSize);

[StructLayout(LayoutKind.Sequential)]
public struct WNF_TYPE_ID
{
	public Guid TypeId;
}

[StructLayout(LayoutKind.Sequential)]
public struct WNF_STATE_NAME
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public uint[] Data;

	public WNF_STATE_NAME(uint Data1, uint Data2) : this()
	{
		uint[] newData = new uint[2];
		newData[0] = Data1;
		newData[1] = Data2;
		Data = newData;
	}
}

public enum FocusAssistState
{
	NOT_SUPPORTED = -2,
	FAILED = -1,
	OFF = 0,
	PRIORITY_ONLY = 1,
	ALARMS_ONLY = 2
};

// Returns the state of Focus Assist if available on this computer
public static FocusAssistState GetFocusAssistState()
{
	try
	{
		WNF_STATE_NAME WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED = new WNF_STATE_NAME(0xA3BF1C75, 0xD83063E);
		uint nBufferSize = (uint)Marshal.SizeOf(typeof(IntPtr));
		IntPtr pStateName = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WNF_STATE_NAME)));
		Marshal.StructureToPtr(WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED, pStateName, false);

		uint nChangeStamp = 0;
		IntPtr pBuffer = IntPtr.Zero;
		bool success = NtQueryWnfStateData(pStateName, IntPtr.Zero, IntPtr.Zero, out nChangeStamp, out pBuffer, ref nBufferSize) == 0;
		Marshal.FreeHGlobal(pStateName);

		if (success)
		{
return (FocusAssistState)pBuffer;
		}
	}
	catch {}

	return FocusAssistState.FAILED;
}
""@
}

if (-not (""WinAPI.Focus"" -as [type]))
{
	Add-Type @Signature
}

while ([WinAPI.Focus]::GetFocusAssistState() -ne ""OFF"")
{
	Start-Sleep -Seconds 600
}

[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null
[Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] | Out-Null

[xml]$ToastTemplate = @""
<toast duration=""Long"">
	<visual>
		<binding template=""ToastGeneric"">
<text>$Localization.CleanupTaskNotificationTitle</text>
<group>
	<subgroup>
		<text hint-style=""body"" hint-wrap=""true"">$Localization.CleanupTaskNotificationEvent</text>
	</subgroup>
</group>
		</binding>
	</visual>
	<audio src=""ms-winsoundevent:notification.default"" />
	<actions>
		<action content=""$([WinAPI.GetStrings]::GetString(12850))"" arguments=""WindowsCleanup:"" activationType=""protocol""/>
		<action content="""" arguments=""dismiss"" activationType=""system""/>
	</actions>
</toast>
""@

$ToastXml = [Windows.Data.Xml.Dom.XmlDocument]::New()
$ToastXml.LoadXml($ToastTemplate.OuterXml)

$ToastMessage = [Windows.UI.Notifications.ToastNotification]::New($ToastXML)
[Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier(""Sophia"").Show($ToastMessage)"
            .Replace("$Localization.CleanupTaskNotificationTitle", "TaskScheduler_WindowsCleanupToast_Title".GetLocalized())
            .Replace("$Localization.CleanupTaskNotificationEvent", "TaskScheduler_WindowsCleanupToast_Description".GetLocalized());

        private readonly string cleanupNotificationToastAction = @"
' https://github.com/farag2/Sophia-Script-for-Windows
' https://t.me/sophia_chat

CreateObject(""Wscript.Shell"").Run ""powershell.exe -ExecutionPolicy Bypass -NoProfile -NoLogo -WindowStyle Hidden -File %SystemRoot%\System32\Tasks\Sophia\Windows_Cleanup_Notification.ps1"", 0";

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskService"/> class.
        /// </summary>
        /// <param name="commonDataService">A service for transferring app data between layers of DI.</param>
        public ScheduledTaskService(ICommonDataService commonDataService)
        {
            taskScheduler = TaskService.Instance;
            this.commonDataService = commonDataService;
        }

        /// <inheritdoc/>
        public IEnumerable<Task?> FindTaskOrDefault(string[] names, bool searchAllFolders = true)
        {
            foreach (var name in names)
            {
                yield return taskScheduler.FindTask(name, searchAllFolders);
            }
        }

        /// <inheritdoc/>
        public Task GetTaskOrDefault(string taskPath)
        {
            return taskScheduler.GetTask(taskPath);
        }

        /// <inheritdoc/>
        public void RegisterWindowsCleanupTask()
        {
            _ = RegisterTask(name: "Sophia\\Windows Cleanup", description: "TaskScheduler_WindowsCleanup_Description".GetLocalized(), action: "powershell.exe", arguments: cleanupTaskAction, username: Environment.UserName, runLevel: TaskRunLevel.Highest);
        }

        /// <inheritdoc/>
        public void RegisterWindowsCleanupNotificationTask()
        {
            var notificationTaskFile = Path.Combine(systemFolder, "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1");
            var notificationToastFile = Path.Combine(systemFolder, "Tasks\\Sophia\\Windows_Cleanup_Notification.vbs");
            File.WriteAllText(notificationTaskFile, cleanupNotificationTaskAction, Encoding.UTF8);
            File.WriteAllText(notificationToastFile, cleanupNotificationToastAction, Encoding.Default);
            _ = RegisterTask(name: "Sophia\\Windows Cleanup Notification", description: "TaskScheduler_WindowsCleanupNotification_Description".GetLocalized(), action: "wscript.exe", arguments: notificationToastFile, username: Environment.UserName, runLevel: TaskRunLevel.Highest, trigger: new DailyTrigger(daysInterval: 30) { StartBoundary = DateTime.Today.AddHours(21) });
        }

        /// <inheritdoc/>
        public void DeleteTaskFolders(string[] taskFolders)
        {
            Array.ForEach(taskFolders, folder =>
            {
                var taskFolder = taskScheduler.GetFolder(folder);

                if (taskFolder?.AllTasks.Any() ?? false)
                {
                    foreach (var task in taskFolder.AllTasks)
                    {
                        taskFolder.DeleteTask(task.Name, false);
                    }
                }

                if (taskFolder is not null)
                {
                    taskScheduler.RootFolder.DeleteFolder(folder, false);
                }
            });
        }

        /// <inheritdoc/>
        public void RemoveExtensionsFilesFromTaskFolder(string taskFolder)
        {
            var files = Directory.EnumerateFiles(Path.Combine(systemFolder, $"Tasks\\{taskFolder}"));

            foreach (var file in files.Where(f => !string.IsNullOrEmpty(Path.GetExtension(f))))
            {
                File.Delete(file);
            }
        }

        private Task RegisterTask(string name, string description, string action, string arguments, string username, TaskRunLevel runLevel, Trigger? trigger = null)
        {
            var task = TaskService.Instance.NewTask();

            if (trigger is not null)
            {
                task.Triggers.Add(trigger);
            }

            task.Actions.Add(action, arguments);
            task.Principal.UserId = username;
            task.Principal.RunLevel = runLevel;
            task.Settings.Compatibility = TaskCompatibility.V2_2;
            task.Settings.StartWhenAvailable = true;
            task.RegistrationInfo.Author = commonDataService.GetName();
            task.RegistrationInfo.Description = description;
            return TaskService.Instance.RootFolder.RegisterTaskDefinition(name, task);
        }
    }
}
