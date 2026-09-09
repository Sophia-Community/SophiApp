// <copyright file="ScheduledTaskService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.IO;
    using System.Text;
    using Microsoft.Win32.TaskScheduler;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <inheritdoc/>
    public class ScheduledTaskService : IScheduledTaskService
    {
        private readonly TaskService taskScheduler;
        private readonly FileInfo cleanupPsFile = new (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.ps1"));
        private readonly FileInfo notificationPsFile = new (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1"));
        private readonly FileInfo softwareDistributionPsFile = new (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.ps1"));
        private readonly FileInfo tempPsFile = new (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.ps1"));
        private readonly string cleanupTaskPS = @"# https://github.com/farag2/Sophia-Script-for-Windows
# https://t.me/sophia_chat

Get-Process -Name cleanmgr, Dism, DismHost | Stop-Process -Force

$ProcessInfo = New-Object -TypeName System.Diagnostics.ProcessStartInfo
$ProcessInfo.FileName = ""$env:SystemRoot\System32\cleanmgr.exe""
$ProcessInfo.Arguments = ""/sagerun:1337""
$ProcessInfo.UseShellExecute = $true
$ProcessInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Minimized

$Process = New-Object -TypeName System.Diagnostics.Process
$Process.StartInfo = $ProcessInfo
$Process.Start() | Out-Null

Start-Sleep -Seconds 3

$ProcessInfo = New-Object -TypeName System.Diagnostics.ProcessStartInfo
$ProcessInfo.FileName = ""$env:SystemRoot\System32\Dism.exe""
$ProcessInfo.Arguments = ""/Online /English /Cleanup-Image /StartComponentCleanup /NoRestart""
$ProcessInfo.UseShellExecute = $true
$ProcessInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Minimized

$Process = New-Object -TypeName System.Diagnostics.Process
$Process.StartInfo = $ProcessInfo
$Process.Start() | Out-Null";

        private readonly string notificationTaskPS = @"# https://github.com/farag2/Sophia-Script-for-Windows
# https://t.me/sophia_chat

# Get Quiet Hours status
$CompilerParameters                  = [System.CodeDom.Compiler.CompilerParameters]::new(""System.dll"")
$CompilerParameters.TempFiles        = [System.CodeDom.Compiler.TempFileCollection]::new($env:TEMP, $false)
$CompilerParameters.GenerateInMemory = $true
$Signature = @{
	Namespace          = ""WinAPI""
	Name               = ""QuietHours""
	Language           = ""CSharp""
	CompilerParameters = $CompilerParameters
	MemberDefinition   = @""
[DllImport(""ntdll.dll"")]
private static extern uint NtQueryWnfStateData(
	ref ulong StateName,
	IntPtr TypeId,
	IntPtr ExplicitScope,
	out uint ChangeStamp,
	out int Buffer,
	ref uint BufferSize
);

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

// WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED
public static int GetState()
{
	ulong stateName = 0x0D83063EA3BF1C75;
	uint size = sizeof(int);
	uint stamp;
	int state;
	uint status = NtQueryWnfStateData(ref stateName, IntPtr.Zero, IntPtr.Zero, out stamp, out state, ref size);
	return (status == 0) ? state : -1;
}
""@
}

if (-not (""WinAPI.QuietHours"" -as [type]))
{
	Add-Type @Signature
}

# Wait until it will be 0
while ([WinAPI.QuietHours]::GetState() -ne 0)
{
	Start-Sleep -Seconds 600
}

[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null
[Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] | Out-Null

[xml]$ToastTemplate = @""
<toast duration=""Long"">
	<visual>
		<binding template=""ToastGeneric"">
			<text>#TaskScheduler_WindowsCleanupToast_Title#</text>
			<group>
				<subgroup>
					<text hint-style=""body"" hint-wrap=""true"">#TaskScheduler_WindowsCleanupToast_Description#</text>
				</subgroup>
			</group>
		</binding>
	</visual>
	<audio src=""ms-winsoundevent:notification.default"" />
	<actions>
		<action content=""#TaskScheduler_WindowsCleanupToast_Run#"" arguments=""WindowsCleanup:"" activationType=""protocol""/>
		<action content="""" arguments=""dismiss"" activationType=""system""/>
	</actions>
</toast>
""@

$ToastXml = [Windows.Data.Xml.Dom.XmlDocument]::New()
$ToastXml.LoadXml($ToastTemplate.OuterXml)

$ToastMessage = [Windows.UI.Notifications.ToastNotification]::New($ToastXML)
[Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier(""Sophia"").Show($ToastMessage)"
    .Replace("#TaskScheduler_WindowsCleanupToast_Title#", "TaskScheduler_WindowsCleanupToast_Title".GetLocalized())
    .Replace("#TaskScheduler_WindowsCleanupToast_Description#", "TaskScheduler_WindowsCleanupToast_Description".GetLocalized())
    .Replace("#TaskScheduler_WindowsCleanupToast_Run#", "TaskScheduler_WindowsCleanupToast_Run".GetLocalized());

        private readonly string softwareDistributionTaskPS = @"# https://github.com/farag2/Sophia-Script-for-Windows
# https://t.me/sophia_chat

# Get Quiet Hours status
$CompilerParameters                  = [System.CodeDom.Compiler.CompilerParameters]::new(""System.dll"")
$CompilerParameters.TempFiles        = [System.CodeDom.Compiler.TempFileCollection]::new($env:TEMP, $false)
$CompilerParameters.GenerateInMemory = $true
$Signature = @{
	Namespace          = ""WinAPI""
	Name               = ""QuietHours""
	Language           = ""CSharp""
	CompilerParameters = $CompilerParameters
	MemberDefinition   = @""
[DllImport(""ntdll.dll"")]
private static extern uint NtQueryWnfStateData(
	ref ulong StateName,
	IntPtr TypeId,
	IntPtr ExplicitScope,
	out uint ChangeStamp,
	out int Buffer,
	ref uint BufferSize
);

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

// WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED
public static int GetState()
{
	ulong stateName = 0x0D83063EA3BF1C75;
	uint size = sizeof(int);
	uint stamp;
	int state;
	uint status = NtQueryWnfStateData(ref stateName, IntPtr.Zero, IntPtr.Zero, out stamp, out state, ref size);
	return (status == 0) ? state : -1;
}
""@
}

if (-not (""WinAPI.QuietHours"" -as [type]))
{
	Add-Type @Signature
}

# Wait until it will be 0
while ([WinAPI.QuietHours]::GetState() -ne 0)
{
	Start-Sleep -Seconds 600
}

# Wait until Windows Update service will stop
(Get-Service -Name wuauserv).WaitForStatus(""Stopped"", ""01:00:00"")
Get-ChildItem -Path $env:SystemRoot\SoftwareDistribution\Download -Recurse -Force | Remove-Item -Recurse -Force
# Remove files which can be removed in a user scope only
Get-ChildItem -Path $env:SystemRoot\SoftwareDistribution\Download -Recurse | Remove-Item -Recurse

[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null
[Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] | Out-Null

[xml]$ToastTemplate = @""
<toast duration=""Long"">
	<visual>
		<binding template=""ToastGeneric"">
			<text>#TaskScheduler_SoftwareDistributionToast_Title#</text>
		</binding>
	</visual>
	<audio src=""ms-winsoundevent:notification.default"" />
</toast>
""@

$ToastXml = [Windows.Data.Xml.Dom.XmlDocument]::New()
$ToastXml.LoadXml($ToastTemplate.OuterXml)

$ToastMessage = [Windows.UI.Notifications.ToastNotification]::New($ToastXML)
[Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier(""Sophia"").Show($ToastMessage)"
    .Replace("#TaskScheduler_SoftwareDistributionToast_Title#", "TaskScheduler_SoftwareDistributionToast_Title".GetLocalized());

        private readonly string tempTaskPS = @"# https://github.com/farag2/Sophia-Script-for-Windows
# https://t.me/sophia_chat

# Get Quiet Hours status
$CompilerParameters                  = [System.CodeDom.Compiler.CompilerParameters]::new(""System.dll"")
$CompilerParameters.TempFiles        = [System.CodeDom.Compiler.TempFileCollection]::new($env:TEMP, $false)
$CompilerParameters.GenerateInMemory = $true
$Signature = @{
	Namespace          = ""WinAPI""
	Name               = ""QuietHours""
	Language           = ""CSharp""
	CompilerParameters = $CompilerParameters
	MemberDefinition   = @""
[DllImport(""ntdll.dll"")]
private static extern uint NtQueryWnfStateData(
	ref ulong StateName,
	IntPtr TypeId,
	IntPtr ExplicitScope,
	out uint ChangeStamp,
	out int Buffer,
	ref uint BufferSize
);

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

// WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED
public static int GetState()
{
	ulong stateName = 0x0D83063EA3BF1C75;
	uint size = sizeof(int);
	uint stamp;
	int state;
	uint status = NtQueryWnfStateData(ref stateName, IntPtr.Zero, IntPtr.Zero, out stamp, out state, ref size);
	return (status == 0) ? state : -1;
}
""@
}

if (-not (""WinAPI.QuietHours"" -as [type]))
{
	Add-Type @Signature
}

# Wait until it will be 0
while ([WinAPI.QuietHours]::GetState() -ne 0)
{
	Start-Sleep -Seconds 600
}

# Run the task
Get-ChildItem -Path $env:TEMP -Recurse -Force | Where-Object -FilterScript {$_.CreationTime -lt (Get-Date).AddDays(-1)} | Remove-Item -Recurse -Force

# Unnecessary folders to remove
$Paths = @(
	# Get ""C:\$WinREAgent"" path because we need to open brackets for $env:SystemDrive but not for $WinREAgent
	(-join (""$env:SystemDrive\"", '$WinREAgent')),
	(-join (""$env:SystemDrive\"", '$SysReset')),
	(-join (""$env:SystemDrive\"", '$Windows.~WS')),
	(-join (""$env:SystemDrive\"", '$GetCurrent')),
	""$env:SystemDrive\ESD"",
	""$env:SystemDrive\Intel"",
	""$env:SystemDrive\PerfLogs"",
	""$env:SystemRoot\ServiceProfiles\NetworkService\AppData\Local\Temp"",
	""$env:LOCALAPPDATA\CrashDumps""
)

if ((Get-ChildItem -Path $env:SystemDrive\Recovery -Force | Where-Object -FilterScript {$_.Name -eq ""ReAgentOld.xml""}).FullName)
{
	$Paths += ""$env:SystemDrive\Recovery""
}
Remove-Item -Path $Paths -Recurse -Force

# C:\WINDOWS\System32\config\systemprofile\AppData\Local with garbage tw-*.tmp files
# https://www.elevenforum.com/t/folder-c-windows-system32-config-systemprofile-app-data-local-running-full-with-empty-folders-after-latest-upgrade-to-25h2.40624/
Get-ChildItem -Path ""$env:SystemRoot\System32\config\systemprofile\AppData\Local\tw-*.tmp"" -Force | Remove-Item -Force

[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null
[Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] | Out-Null

[xml]$ToastTemplate = @""
<toast duration=""Long"">
	<visual>
		<binding template=""ToastGeneric"">
			<text>#TaskScheduler_TempTaskToast_Title#</text>
		</binding>
	</visual>
	<audio src=""ms-winsoundevent:notification.default"" />
</toast>
""@

$ToastXml = [Windows.Data.Xml.Dom.XmlDocument]::New()
$ToastXml.LoadXml($ToastTemplate.OuterXml)

$ToastMessage = [Windows.UI.Notifications.ToastNotification]::New($ToastXML)
[Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier(""Sophia"").Show($ToastMessage)"
    .Replace("#TaskScheduler_TempTaskToast_Title#", "TaskScheduler_TempTaskToast_Title".GetLocalized());

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskService"/> class.
        /// </summary>
        public ScheduledTaskService() => taskScheduler = TaskService.Instance;

        /// <inheritdoc/>
        public Task? FindTaskOrDefault(string name, bool searchAllFolders = true) => taskScheduler.FindTask(name, searchAllFolders);

        /// <inheritdoc/>
        public Task? GetTaskOrDefault(string taskPath) => taskScheduler.GetTask(taskPath);

        /// <inheritdoc/>
        public void RegisterCleanupTask()
        {
            cleanupPsFile.Directory?.Create();
            File.WriteAllText(cleanupPsFile.FullName, cleanupTaskPS, Encoding.UTF8);

            // Create "Windows Cleanup" task
            // We use conhost.exe with an undocumented "--headless" argument to suppress console appearing
            _ = RegisterTask(
                name: "Sophia\\Windows Cleanup",
                description: string.Format("TaskScheduler_WindowsCleanup_Description".GetLocalized(), Environment.UserName),
                action: "conhost.exe",
                arguments: $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.ps1")}",
                runLevel: TaskRunLevel.Highest);
        }

        /// <inheritdoc/>
        public void RegisterCleanupNotificationTask()
        {
            notificationPsFile.Directory?.Create();
            File.WriteAllText(notificationPsFile.FullName, notificationTaskPS, Encoding.UTF8);

            // Create "Windows Cleanup Notification" task
            // We use conhost.exe with an undocumented "--headless" argument to suppress console appearing
            _ = RegisterTask(
                name: "Sophia\\Windows Cleanup Notification",
                description: string.Format("TaskScheduler_WindowsCleanupNotification_Description".GetLocalized(), Environment.UserName),
                action: "conhost.exe",
                arguments: $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1")}",
                runLevel: TaskRunLevel.Highest,
                trigger: new DailyTrigger(daysInterval: 30) { StartBoundary = DateTime.Today.AddHours(21) });
        }

        /// <inheritdoc/>
        public void UnregisterCleanupTask()
        {
            cleanupPsFile.FullName.TryDelete();
            UnregisterTask("Sophia\\Windows Cleanup");
        }

        /// <inheritdoc/>
        public void UnregisterCleanupNotificationTask()
        {
            notificationPsFile.FullName.TryDelete();
            UnregisterTask("Sophia\\Windows Cleanup Notification");
        }

        /// <inheritdoc/>
        public void UnregisterOneDriveTasks() => taskScheduler.FindAllTasks(task => task.Name.Contains("OneDrive")).ToList().ForEach(task => UnregisterTask(task.Path));

        /// <inheritdoc/>
        public void RegisterSoftwareDistributionTask()
        {
            softwareDistributionPsFile.Directory?.Create();
            File.WriteAllText(softwareDistributionPsFile.FullName, softwareDistributionTaskPS, Encoding.UTF8);

            // Create "SoftwareDistribution" task
            // We use conhost.exe with an undocumented "--headless" argument to suppress console appearing
            _ = RegisterTask(
                name: "Sophia\\SoftwareDistribution",
                description: string.Format("TaskScheduler_SoftwareDistribution_Description".GetLocalized(), Environment.UserName),
                action: "conhost.exe",
                arguments: $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.ps1")}",
                runLevel: TaskRunLevel.Highest,
                trigger: new DailyTrigger(daysInterval: 90) { StartBoundary = DateTime.Today.AddHours(21) });
        }

        /// <inheritdoc/>
        public void UnregisterSoftwareDistributionTask()
        {
            softwareDistributionPsFile.FullName.TryDelete();
            UnregisterTask("Sophia\\SoftwareDistribution");
        }

        /// <inheritdoc/>
        public void RegisterTempTask()
        {
            tempPsFile.Directory?.Create();
            File.WriteAllText(tempPsFile.FullName, tempTaskPS, Encoding.UTF8);

            _ = RegisterTask(
                name: "Sophia\\Temp",
                description: string.Format("TaskScheduler_TempTask_Description".GetLocalized(), Environment.UserName),
                action: "conhost.exe",
                arguments: $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.ps1")}",
                runLevel: TaskRunLevel.Highest,
                trigger: new DailyTrigger(daysInterval: 60) { StartBoundary = DateTime.Today.AddHours(21) });
        }

        /// <inheritdoc/>
        public void UnregisterTempTask()
        {
            tempPsFile.FullName.TryDelete();
            UnregisterTask("Sophia\\Temp");
        }

        /// <inheritdoc/>
        public void SetState(Task? task, bool enabled)
        {
            if (task is not null)
            {
                task.Enabled = enabled;
            }
        }

        /// <inheritdoc/>
        public void TryStop(Task? task)
        {
            try
            {
                task?.Stop();
            }
            catch
            {
                // Do nothing.
            }
        }

        /// <inheritdoc/>
        public void TryDeleteTaskFolder(string name)
        {
            if (!taskScheduler.GetFolder(name)?.AllTasks.Any() ?? false)
            {
                taskScheduler.RootFolder.DeleteFolder(name, false);
            }
        }

        private Task RegisterTask(string name, string description, string action, string arguments, TaskRunLevel runLevel, Trigger? trigger = null)
        {
            var task = taskScheduler.NewTask();

            if (trigger is not null)
            {
                task.Triggers.Add(trigger);
            }

            task.Actions.Add(action, arguments);
            task.Principal.UserId = Environment.UserName;
            task.Principal.RunLevel = runLevel;
            task.Settings.Compatibility = TaskCompatibility.V2_2;
            task.Settings.StartWhenAvailable = true;
            task.RegistrationInfo.Author = "Team Sophia";
            task.RegistrationInfo.Description = description;
            return taskScheduler.RootFolder.RegisterTaskDefinition(name, task);
        }

        private void UnregisterTask(string path) => taskScheduler.RootFolder.DeleteTask(path, false);
    }
}
