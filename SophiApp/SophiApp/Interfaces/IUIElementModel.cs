using SophiApp.Commons;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IUIElementModel
    {
        bool ActualState { get; set; }
        string Description { get; set; }
        string Header { get; set; }
        int Id { get; set; }
        Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }
        Dictionary<UILanguage, string> LocalizedHeaders { get; set; }
        bool State { get; set; }
        string Tag { get; set; }

        void ChangeActualState();

        void SetLocalizationTo(UILanguage language);
    }
}