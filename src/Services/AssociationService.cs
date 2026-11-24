// <copyright file="AssociationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Xml.Linq;

    /// <inheritdoc/>
    public class AssociationService : IAssociationService
    {
        private const string Classes = "Software\\Classes";
        private const string Toasts = "Software\\Microsoft\\Windows\\CurrentVersion\\ApplicationAssociationToasts";
        private const string FileExtensions = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts";
        private const string Protocols = "Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations";

        // ASSOCSTR_FRIENDLYAPPNAME
        private const uint FriendlyAppName = 4;

        // The default apps and their hashes are written within one minute, so the end of a minute is skipped
        private const int LastSafeSecond = 50;

        // A command line is limited to 32767 characters, and an encoded command takes almost three characters for one
        private const int MaxCommandLength = 9000;

        // The default apps of a batch are written once more if the minute has changed while they were being written
        private const int WriteAttempts = 2;

        private readonly IProcessService processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationService"/> class.
        /// </summary>
        /// <param name="processService">A service for working with Windows process API.</param>
        public AssociationService(IProcessService processService)
        {
            this.processService = processService;
        }

        // Windows lets only its own programs change the default app of .pdf, http and https, and not the ones known for scripting.
        // A copy of Windows PowerShell under another name is let through, so the default apps are written by it
        private static string RegistryWriter => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell\\v1.0\\powershell_temp.exe");

        /// <inheritdoc/>
        public string GetDefaultApp(string extension)
        {
            uint size = 0;
            _ = AssocQueryString(0, FriendlyAppName, extension, null, null, ref size);

            if (size == 0)
            {
                return string.Empty;
            }

            var name = new StringBuilder((int)size);
            return AssocQueryString(0, FriendlyAppName, extension, null, name, ref size) == 0 ? name.ToString() : string.Empty;
        }

        /// <inheritdoc/>
        public IReadOnlyList<(string Extension, CustomizationException Error)> SetDefaultApps(IReadOnlyList<AssociationRecord> records, bool skipCurrent)
        {
            var failures = new List<(string Extension, CustomizationException Error)>();
            var choices = new List<UserChoice>();
            var packaged = GetPackagedProgIds();
            var registeredApps = GetRegisteredApps();

            for (var index = 0; index < records.Count; index++)
            {
                var record = records[index];

                if (records.Count > 1)
                {
                    App.Status.SetStep("Panel_Step_ImportingAssociation", index + 1, records.Count);
                }

                // A packaged app, an app saved without a path, and an app that is registered on this PC are set by their ProgId:
                // an app registers it by itself when it is installed, with its own command, icon and name.
                // The saved command is needed only for a ProgId that does not exist here
                var byProgId = packaged.Contains(record.ProgId)
                    || record.ProgramPath.Length == 0
                    || (record.ProgId.Length > 0 && Registry.ClassesRoot.SubKeyExists(record.ProgId));
                var program = byProgId ? record.ProgId : record.ProgramPath;
                var icon = byProgId ? string.Empty : record.Icon;

                if (AssociationHelper.GetProblem(program, record.Extension, icon) is var (resourceKey, argument))
                {
                    failures.Add((record.Extension, new CustomizationException(exceptionMessage: $"{resourceKey}: {argument}", resourceKey: resourceKey) { Arguments = [argument] }));
                    continue;
                }

                if (skipCurrent && IsDefaultApp(program, record.Extension))
                {
                    continue;
                }

                choices.Add(Register(program, record.Extension, icon, registeredApps));
            }

            if (choices.Count == 0)
            {
                return failures;
            }

            File.Copy(Path.Combine(Path.GetDirectoryName(RegistryWriter)!, "powershell.exe"), RegistryWriter, overwrite: true);

            try
            {
                foreach (var failed in WriteUserChoices(choices))
                {
                    failures.Add((failed.Extension, new CustomizationException(exceptionMessage: $"The default app {failed.ProgId} of {failed.Extension} was not written to HKCU\\{failed.SubKey}", resourceKey: "UIModel_Exception_ApplyFailed")));
                }
            }
            finally
            {
                RegistryWriter.TryDelete();
            }

            return failures;
        }

        /// <inheritdoc/>
        public void Export(string file)
        {
            App.Status.SetStep("Panel_Step_ExportingAssociations");
            var list = Path.Combine(Path.GetTempPath(), "Application_Associations.xml");

            try
            {
                // Windows writes the default apps of the user as an XML file
                var dism = processService.WaitForExit(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Dism.exe"), $"/Online /Export-DefaultAppAssociations:\"{list}\"");

                if (dism.ExitCode != 0 || !File.Exists(list))
                {
                    throw new CustomizationException(exceptionMessage: $"DISM could not export the associations, exit code {dism.ExitCode}", resourceKey: "UIModel_SetAssociation_ExportFailed") { Arguments = [dism.ExitCode] };
                }

                var packaged = GetPackagedProgIds();
                var records = new JsonArray();

                foreach (var association in XDocument.Load(list).Descendants("Association"))
                {
                    var progId = association.Attribute("ProgId")?.Value ?? string.Empty;

                    // A packaged app is set by its ProgId alone: it needs neither a path nor an icon
                    var (command, program) = packaged.Contains(progId) ? (string.Empty, string.Empty) : GetOpenCommand(progId);
                    var icon = packaged.Contains(progId) ? string.Empty : GetIcon(progId, program);

                    records.Add(new JsonObject
                    {
                        ["ProgId"] = progId,
                        ["ProgramPath"] = command,
                        ["Extension"] = association.Attribute("Identifier")?.Value ?? string.Empty,
                        ["Icon"] = icon,
                    });
                }

                File.WriteAllText(file, records.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
            finally
            {
                list.TryDelete();
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<AssociationRecord> ReadExported(string file)
        {
            try
            {
                var records = (JsonNode.Parse(File.ReadAllText(file)) as JsonArray ?? throw new JsonException("The file is not a list"))
                    .Select(node => new AssociationRecord(
                        ProgId: node?["ProgId"]?.GetValue<string>() ?? string.Empty,
                        ProgramPath: node?["ProgramPath"]?.GetValue<string>() ?? string.Empty,
                        Extension: node?["Extension"]?.GetValue<string>() ?? string.Empty,
                        Icon: node?["Icon"]?.GetValue<string>() ?? string.Empty))
                    .Where(record => record.Extension.Length > 0 && (record.ProgId.Length > 0 || record.ProgramPath.Length > 0))
                    .ToList();

                return records.Count > 0 ? records : throw new JsonException("The file has no associations");
            }
            catch (JsonException ex) when (ex.LineNumber is not null)
            {
                // The text of the file is broken: the line tells where to look, for a user who edited the file by hand
                throw new CustomizationException(exceptionMessage: $"{file} is not valid JSON: {ex.Message}", resourceKey: "UIModel_SetAssociation_FileNotJson") { Arguments = [file, ex.LineNumber.Value + 1] };
            }
            catch (Exception ex) when (ex is JsonException || ex is InvalidOperationException || ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new CustomizationException(exceptionMessage: $"{file} is not a file of exported associations: {ex.Message}", resourceKey: "UIModel_SetAssociation_FileNotValid") { Arguments = [file] };
            }
        }

        // The ProgIds of the packaged apps that are installed
        private static HashSet<string> GetPackagedProgIds()
        {
            using var key = Registry.ClassesRoot.OpenSubKey("Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\PackageRepository\\Extensions\\ProgIDs");
            return new HashSet<string>(key?.GetSubKeyNames() ?? [], StringComparer.OrdinalIgnoreCase);
        }

        // The command a registered app opens a file with, and its executable file, if the file exists
        private static (string Command, string Program) GetOpenCommand(string progId)
        {
            foreach (var hive in new[] { Registry.CurrentUser, Registry.LocalMachine })
            {
                var command = (hive.GetSubKeyValue($"{Classes}\\{progId}\\shell\\open\\command", string.Empty) as string ?? string.Empty).Trim();
                var (program, _, isFile) = AssociationHelper.ParseProgram(command);

                if (isFile && program.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(program))
                {
                    return (command, program);
                }
            }

            return (string.Empty, string.Empty);
        }

        // The icon of a registered app, if its file exists; otherwise the first icon of the program
        private static string GetIcon(string progId, string program)
        {
            foreach (var hive in new[] { Registry.CurrentUser, Registry.LocalMachine })
            {
                var icon = (hive.GetSubKeyValue($"{Classes}\\{progId}\\DefaultIcon", string.Empty) as string ?? string.Empty).Trim();

                if (icon.Length > 0 && File.Exists(AssociationHelper.GetIconFile(icon)))
                {
                    return icon;
                }
            }

            return program.Length > 0 ? $"{program},0" : string.Empty;
        }

        // The ProgId a program gets, or has, for an extension
        private static string GetProgId(string program, string extension)
        {
            var (target, _, isFile) = AssociationHelper.ParseProgram(program);
            return isFile ? Path.GetFileNameWithoutExtension(target) + extension.ToUpperInvariant() : target;
        }

        private static string GetUserChoiceKey(string extension)
            => AssociationHelper.IsFileExtension(extension) ? $"{FileExtensions}\\{extension}\\UserChoice" : $"{Protocols}\\{extension}\\UserChoice";

        // The program is what the user has already chosen for the extension
        private static bool IsDefaultApp(string program, string extension)
            => GetProgId(program, extension).Equals(Registry.CurrentUser.GetSubKeyValue(GetUserChoiceKey(extension), "ProgId") as string, StringComparison.OrdinalIgnoreCase);

        // Windows does not show "You have a new app that can open this type of file" for the app
        private static void HideToast(string progId, string extension)
            => Registry.CurrentUser.CreateKeyAndSetValue(Toasts, $"{progId}_{extension}", 0, RegistryValueKind.DWord);

        // A value without data: only its name matters
        private static void AddOpenWithProgId(string subKey, string progId)
        {
            using var key = Registry.CurrentUser.CreateSubKey(subKey);
            key.SetValue(progId, Array.Empty<byte>(), RegistryValueKind.None);
        }

        /// <summary>
        /// Writes everything a default app needs except the choice of the user itself: how the program opens a file, its icon,
        /// its place in "Open with", and the marks that keep Windows from offering other apps.
        /// </summary>
        /// <returns>The choice of the user to write.</returns>
        private static UserChoice Register(string program, string extension, string icon, List<(Microsoft.Win32.RegistryKey Hive, string Capabilities)> registeredApps)
        {
            var (target, command, isFile) = AssociationHelper.ParseProgram(program);
            var isFileExtension = AssociationHelper.IsFileExtension(extension);

            // A program that is not registered gets a ProgId of its own
            var progId = GetProgId(program, extension);
            var systemProgId = Registry.LocalMachine.GetSubKeyValue($"SOFTWARE\\Classes\\{extension}", string.Empty) as string ?? string.Empty;

            // The apps Windows could offer instead: it is told that the user has already seen them
            var knownProgIds = new List<string> { progId };

            if (isFile)
            {
                // How the program opens a file
                var openCommand = command ?? $"\"{target}\" \"%1\"";
                Registry.CurrentUser.CreateKeyAndSetValue($"{Classes}\\{progId}\\shell\\open\\command", string.Empty, openCommand, RegistryValueKind.String);
                Registry.CurrentUser.CreateKeyAndSetValue($"{Classes}\\Applications\\{Path.GetFileName(target)}\\shell\\open\\command", string.Empty, openCommand, RegistryValueKind.String);
            }

            if (icon.Trim().Length > 0)
            {
                Registry.CurrentUser.CreateKeyAndSetValue($"{Classes}\\{progId}\\DefaultIcon", string.Empty, Environment.ExpandEnvironmentVariables(icon.Trim()), RegistryValueKind.String);
            }

            if (isFileExtension)
            {
                // A ProgId like "Applications\app.exe" is also known by its last part
                knownProgIds.Add(Path.GetFileName(progId));

                // An extension Windows knows nothing about gets the program as its class
                if (systemProgId.Length == 0)
                {
                    Registry.CurrentUser.CreateKeyAndSetValue($"{Classes}\\{extension}", string.Empty, progId, RegistryValueKind.String);
                }

                // The program and the app of Windows are both offered in "Open with"
                AddOpenWithProgId($"{Classes}\\{extension}\\OpenWithProgids", progId);

                if (systemProgId.Length > 0)
                {
                    AddOpenWithProgId($"{FileExtensions}\\{extension}\\OpenWithProgids", systemProgId);
                    knownProgIds.Add(systemProgId);
                }
            }

            // The extension has an app of Windows: the choice of the user replaces it for new users of the PC too
            if (systemProgId.Length > 0)
            {
                Registry.Users.CreateKeyAndSetValue(".DEFAULT\\Software\\Microsoft\\Windows\\CurrentVersion\\FileAssociations\\ProgIds", $"_{extension}", 1, RegistryValueKind.DWord);
            }

            knownProgIds.AddRange(HidePackagedApps(progId, extension));

            // Paint opens every kind of picture
            if (Registry.LocalMachine.GetSubKeyValue("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\KindMap", extension) as string == "picture"
                && Registry.LocalMachine.GetSubKeyValue("SOFTWARE\\Classes\\PBrush\\CLSID", string.Empty) is not null)
            {
                knownProgIds.Add("PBrush");
            }

            // What the registered apps of the PC and of the user open the extension or the protocol with
            var associations = isFileExtension ? "FileAssociations" : "UrlAssociations";
            knownProgIds.AddRange(registeredApps
                .Select(app => app.Hive.GetSubKeyValue($"{app.Capabilities}\\{associations}", extension) as string)
                .OfType<string>()
                .Where(registered => registered.Length > 0));

            foreach (var known in knownProgIds.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                HideToast(known, extension);
            }

            // Windows itself protects the choice for an extension, not for a protocol
            return new UserChoice(GetUserChoiceKey(extension), progId, extension, Protect: isFileExtension);
        }

        /// <summary>
        /// Takes the packaged apps that can open the extension out of the "Open with" list, so they do not get the files back.
        /// The app that is made the default one is returned to the list.
        /// </summary>
        /// <returns>The ProgIds of the installed packaged apps of the extension.</returns>
        private static List<string> HidePackagedApps(string progId, string extension)
        {
            var installed = new List<string>();
            using var openWith = Registry.ClassesRoot.OpenSubKey($"{extension}\\OpenWithProgids");

            foreach (var packaged in openWith?.GetValueNames().Where(name => name.Contains("AppX", StringComparison.Ordinal)) ?? [])
            {
                // An app that is not installed has no package
                if (Registry.CurrentUser.GetSubKeyValue($"{Classes}\\{packaged}\\Shell\\open", "PackageId") is null)
                {
                    continue;
                }

                if (packaged.Equals(progId, StringComparison.OrdinalIgnoreCase))
                {
                    Registry.CurrentUser.DeleteSubKeyValue($"{Classes}\\{packaged}", "NoOpenWith");
                    Registry.CurrentUser.DeleteSubKeyValue($"{Classes}\\{packaged}", "NoStaticDefaultVerb");
                }
                else
                {
                    Registry.CurrentUser.CreateKeyAndSetValue($"{Classes}\\{packaged}", "NoOpenWith", string.Empty, RegistryValueKind.String);
                }

                installed.Add(packaged);
            }

            return installed;
        }

        // The keys where the registered apps of the PC and of the user tell what they can open
        private static List<(Microsoft.Win32.RegistryKey Hive, string Capabilities)> GetRegisteredApps()
        {
            var apps = new List<(Microsoft.Win32.RegistryKey Hive, string Capabilities)>();

            foreach (var hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
            {
                using var registered = hive.OpenSubKey("Software\\RegisteredApplications");
                apps.AddRange((registered?.GetValueNames() ?? [])
                    .Select(name => registered!.GetValue(name) as string)
                    .OfType<string>()
                    .Where(path => path.Length > 0)
                    .Select(path => (hive, path)));
            }

            return apps;
        }

        // The commands that make the key of a choice anew with the app and its hash: Windows forbids the user to change the values of the key
        private static string GetWriteCommand(UserChoice choice, string hash, string user)
        {
            static string Quote(string text) => $"'{text.Replace("'", "''")}'";

            var commands = new List<string>
            {
                $"$k = {Quote(choice.SubKey)}",
                "[Microsoft.Win32.Registry]::CurrentUser.DeleteSubKey($k, $false)",
                "$c = [Microsoft.Win32.Registry]::CurrentUser.CreateSubKey($k)",
                $"$c.SetValue('ProgId', {Quote(choice.ProgId)})",
                $"$c.SetValue('Hash', {Quote(hash)})",
                "$c.Close()",
            };

            if (choice.Protect)
            {
                // The same protection Windows puts on the key: the user may not change its values
                commands.Add("$c = [Microsoft.Win32.Registry]::CurrentUser.OpenSubKey($k, 'ReadWriteSubTree', 'ChangePermissions, TakeOwnership')");
                commands.Add("$a = [System.Security.AccessControl.RegistrySecurity]::new()");
                commands.Add($"$a.SetSecurityDescriptorSddlForm('O:{user}G:{user}D:AI(D;;DC;;;{user})')");
                commands.Add("$c.SetAccessControl($a)");
                commands.Add("$c.Close()");
            }

            return string.Join("; ", commands);
        }

        [DllImport("shlwapi.dll", EntryPoint = "AssocQueryStringW", CharSet = CharSet.Unicode)]
        private static extern int AssocQueryString(uint flags, uint text, string association, string? extra, StringBuilder? output, ref uint size);

        /// <summary>
        /// Writes the default apps and their hashes. A hash includes the minute its key is written in, so the hashes
        /// are counted for the current minute beforehand, and all the keys are written by one start of the registry writer.
        /// </summary>
        /// <returns>The choices that could not be written.</returns>
        private List<UserChoice> WriteUserChoices(List<UserChoice> choices)
        {
            var user = WindowsIdentity.GetCurrent().User?.Value ?? string.Empty;
            var left = choices;

            for (var attempt = 0; attempt < WriteAttempts && left.Count > 0; attempt++)
            {
                var written = new List<(UserChoice Choice, string Hash, long Minute)>();
                var commands = new StringBuilder();

                foreach (var choice in left)
                {
                    // A new start of the writer begins with a fresh minute
                    if (commands.Length == 0 && DateTime.Now.Second >= LastSafeSecond)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(60 - DateTime.Now.Second));
                    }

                    var minute = written.Count > 0 && commands.Length > 0 ? written[^1].Minute : UserChoiceHash.GetCurrentMinute();
                    var hash = UserChoiceHash.Get(choice.Extension, user, choice.ProgId, minute);
                    _ = commands.Append(GetWriteCommand(choice, hash, user)).Append("; ");
                    written.Add((choice, hash, minute));

                    if (commands.Length >= MaxCommandLength)
                    {
                        RunRegistryWriter(commands);
                    }
                }

                RunRegistryWriter(commands);

                // A key written after the minute has changed has a hash Windows does not accept: it is written once more
                left = written
                    .Where(item => Registry.CurrentUser.GetSubKeyValue(item.Choice.SubKey, "ProgId") as string != item.Choice.ProgId
                        || Registry.CurrentUser.GetSubKeyValue(item.Choice.SubKey, "Hash") as string != item.Hash
                        || UserChoiceHash.GetKeyMinute(item.Choice.SubKey) != item.Minute)
                    .Select(item => item.Choice)
                    .ToList();
            }

            return left;
        }

        private void RunRegistryWriter(StringBuilder commands)
        {
            if (commands.Length == 0)
            {
                return;
            }

            // The commands are passed encoded, so no quote in a name can break them
            var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(commands.ToString()));
            _ = processService.WaitForExit(RegistryWriter, $"-NoProfile -NonInteractive -EncodedCommand {encoded}");
            _ = commands.Clear();
        }

        // The choice of the user to write: the key, the app, and whether the key is protected the way Windows does it
        private sealed record UserChoice(string SubKey, string ProgId, string Extension, bool Protect);
    }
}
