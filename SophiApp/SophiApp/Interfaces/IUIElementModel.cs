using SophiApp.Commons;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IUIElementModel
    {
        string Description { get; set; }
        string Header { get; set; }
        int Id { get; set; }
        Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }
        Dictionary<UILanguage, string> LocalizedHeaders { get; set; }
        bool SystemState { get; set; }
        string Tag { get; set; }
        bool UserState { get; set; }

        void SetLocalizationTo(UILanguage language);

        void SetSystemState();

        void SetUserState();
    }
}