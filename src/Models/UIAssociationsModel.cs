// <copyright file="UIAssociationsModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// The default apps of file extensions and protocols. The user adds a row for each program and extension;
    /// the rows are applied with the other chosen settings. An applied row stays and shows what opens the extension now.
    /// </summary>
    public class UIAssociationsModel : UIModel
    {
        // How many extensions that could not be imported are named in the notice
        private const int ShownExtensions = 12;

        private readonly Func<string, string> accessor;
        private readonly Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>> mutator;

        // The default apps of the file chosen for the import
        private IReadOnlyList<AssociationRecord> imported = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="UIAssociationsModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that returns the name of the app that opens an extension.</param>
        /// <param name="mutator">Method that makes programs the default apps of extensions and returns the reasons of the failures.</param>
        public UIAssociationsModel(UIModelDto dto, string title, string description, Func<string, string> accessor, Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>> mutator)
            : base(dto, title)
        {
            Description = description;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the rows. The list is changed on the UI thread only.
        /// </summary>
        public ObservableCollection<AssociationItem> Items { get; } = [];

        /// <summary>
        /// Gets the method that reads the default apps saved to a file.
        /// </summary>
        public Func<string, IReadOnlyList<AssociationRecord>>? ImportReader { get; init; }

        /// <summary>
        /// Gets the method that sets the default apps read from a file and returns the extensions it could not set.
        /// </summary>
        public Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>>? Importer { get; init; }

        /// <summary>
        /// Gets the method that saves the default apps to a file.
        /// </summary>
        public Action<string>? Exporter { get; init; }

        /// <summary>
        /// Gets the file the default apps are going to be saved to, or an empty string.
        /// </summary>
        public string ExportFile { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the text that tells where the default apps are going to be saved.
        /// </summary>
        public string ExportText => string.Format("Association_ExportChosen".GetLocalized(), ExportFile);

        /// <summary>
        /// Gets the visibility of <see cref="ExportText"/>.
        /// </summary>
        public Visibility ExportVisibility => ExportFile.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the file the default apps are going to be taken from, or an empty string.
        /// </summary>
        public string ImportFile { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the text that tells which file the default apps are going to be taken from and how many there are.
        /// </summary>
        public string ImportText => string.Format("Association_ImportChosen".GetLocalized(), ImportFile, imported.Count);

        /// <summary>
        /// Gets the visibility of <see cref="ImportText"/>.
        /// </summary>
        public Visibility ImportVisibility => ImportFile.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets a value indicating whether there is a row, an export or an import to apply.
        /// </summary>
        public override bool IsChanged => ExportFile.Length > 0 || ImportFile.Length > 0 || Items.Any(IsChosen);

        /// <inheritdoc/>
        public override string ChosenValue => string.Join("; ", new[] { ExportFile.Length > 0 ? $"export to {ExportFile}" : string.Empty, ImportFile.Length > 0 ? $"import {imported.Count} from {ImportFile}" : string.Empty }
            .Concat(Items.Where(IsChosen).Select(item => $"{item.Extension} -> {item.Program}"))
            .Where(text => text.Length > 0));

        /// <summary>
        /// Chooses a file to save the default apps to. An empty path takes the choice back.
        /// </summary>
        /// <param name="file">The path of the JSON file to write.</param>
        public void ChooseExport(string file)
        {
            ExportFile = file;
            OnPropertyChanged(nameof(ExportText));
            OnPropertyChanged(nameof(ExportVisibility));
        }

        /// <summary>
        /// Chooses a file to take the default apps from. A file that is not a list of default apps is not chosen, and the reason is shown.
        /// An empty path takes the choice back.
        /// </summary>
        /// <param name="file">The path of the JSON file.</param>
        public void ChooseImport(string file)
        {
            try
            {
                imported = file.Length > 0 && ImportReader is not null ? ImportReader.Invoke(file) : [];
                ImportFile = imported.Count > 0 ? file : string.Empty;
                ShowProblem();
            }
            catch (CustomizationException ex)
            {
                (imported, ImportFile) = ([], string.Empty);
                ShowNotice(ex.GetText());
                App.Logger.LogModelGetStateException(Name, ex);
            }

            OnPropertyChanged(nameof(ImportText));
            OnPropertyChanged(nameof(ImportVisibility));
        }

        /// <summary>
        /// Adds an empty row.
        /// </summary>
        /// <returns>The new row.</returns>
        public AssociationItem Add()
        {
            var item = new AssociationItem();
            Items.Add(item);
            return item;
        }

        /// <summary>
        /// Removes a row. A row that was applied leaves the app in Windows as it is.
        /// </summary>
        /// <param name="item">One of <see cref="Items"/>.</param>
        public void Remove(AssociationItem item)
        {
            _ = Items.Remove(item);
            ShowProblem();
        }

        /// <summary>
        /// Takes what the user typed or picked for a row. If a row cannot be applied, the reason is shown under the setting.
        /// </summary>
        /// <param name="item">One of <see cref="Items"/>.</param>
        /// <param name="program">The path to the program, the whole command with it, or a ProgId.</param>
        /// <param name="extension">The extension with the dot, or the name of the protocol.</param>
        /// <param name="icon">The path to the icon, or an empty string.</param>
        public void Update(AssociationItem item, string program, string extension, string icon)
        {
            item.Program = program.Trim();
            item.Icon = icon.Trim();

            if (item.Extension != extension.Trim())
            {
                item.Extension = extension.Trim().ToLowerInvariant();
                item.CurrentApp = GetCurrentApp(item.Extension);
            }

            ShowProblem();
        }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                // The rows that were not applied are a choice that is canceled by now
                // The export and the import are done or canceled by now
                ChooseExport(string.Empty);
                (imported, ImportFile) = ([], string.Empty);
                OnPropertyChanged(nameof(ImportText));
                OnPropertyChanged(nameof(ImportVisibility));

                var rows = Items.ToList();
                var canceled = rows.Where(item => !item.IsApplied).ToList();
                canceled.ForEach(item => item.MarkApplied());
                _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => canceled.ForEach(item => Items.Remove(item)));

                foreach (var item in rows.Except(canceled))
                {
                    item.CurrentApp = GetCurrentApp(item.Extension);
                }

                App.Logger.LogModelState(Name, rows.Count - canceled.Count);
                IsEnabled = true;
                ShowNotice(string.Empty);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelGetStateException(Name, ex);
            }
        }

        /// <inheritdoc/>
        public override void SetState()
        {
            var notices = new List<string>();

            // The export goes first: it saves the default apps as they are before the changes
            if (ExportFile.Length > 0)
            {
                Run(() => Exporter?.Invoke(ExportFile), $"export to {ExportFile}", notices);
            }

            if (ImportFile.Length > 0)
            {
                Run(
                    () =>
                    {
                        var failed = Importer?.Invoke(imported) ?? [];

                        if (failed.Count > 0)
                        {
                            // A long list is cut: the log has it whole
                            var shown = string.Join(", ", failed.Take(ShownExtensions)) + (failed.Count > ShownExtensions ? "…" : string.Empty);
                            notices.Add(string.Format("UIModel_SetAssociation_ImportIncomplete".GetLocalized(), failed.Count, imported.Count, shown));
                        }
                    },
                    $"import from {ImportFile}",
                    notices);
            }

            // All rows are applied at once: the default apps are written together
            var rows = Items.Where(IsChosen).ToList();

            if (rows.Count > 0)
            {
                Run(
                    () => notices.AddRange(mutator.Invoke(rows.Select(item => new AssociationRecord(string.Empty, item.Program, item.Extension, item.Icon)).ToList())),
                    string.Join("; ", rows.Select(item => $"{item.Extension} -> {item.Program}")),
                    notices);

                // The rows stay: they show what opens the extensions after the change
                rows.ForEach(item => item.MarkApplied());
            }

            if (notices.Count > 0)
            {
                SetApplyFailure(string.Join(" ", notices.Distinct()));
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text) || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase);

        private static (string ResourceKey, string Argument)? GetProblem(AssociationItem item)
            => AssociationHelper.GetProblem(item.Program, item.Extension, item.Icon);

        private static bool IsChosen(AssociationItem item) => item.IsFilled && !item.IsApplied && GetProblem(item) is null;

        // Does a part of the setting; a failure becomes a notice and does not stop the other parts
        private void Run(Action action, string value, List<string> notices)
        {
            try
            {
                action.Invoke();
            }
            catch (CustomizationException ex)
            {
                notices.Add(ex.GetText());
                App.Logger.LogModelSetStateException(ex, Name, value);
            }
            catch (Exception ex)
            {
                notices.Add(ApplyFailedText);
                App.Logger.LogModelSetStateException(ex, Name, value);
            }
        }

        private string GetCurrentApp(string extension)
        {
            try
            {
                return extension.Length > 0 ? accessor.Invoke(extension) : string.Empty;
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException($"Reading the default app of {extension}", ex);
                return string.Empty;
            }
        }

        // The first thing that keeps a row from being applied
        private void ShowProblem()
        {
            var problem = Items.Where(item => !item.IsApplied).Select(GetProblem).FirstOrDefault(found => found is not null);
            ShowNotice(problem is var (resourceKey, argument) ? string.Format(resourceKey.GetLocalized(), argument) : string.Empty);
        }
    }
}
