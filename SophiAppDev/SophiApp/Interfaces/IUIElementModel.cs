using SophiApp.Commons;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IUIElementModel
    {
        string Description { get; set; }
        Dictionary<UILanguage, string> Descriptions { get; set; }
        bool HasParent { get; set; }
        string Header { get; set; }
        Dictionary<UILanguage, string> Headers { get; set; }
        int Id { get; set; }
        bool IsChecked { get; set; }
        bool SystemState { get; set; }
        string Tag { get; set; }
        bool UserState { get; set; }

        void SetLocalizationTo(UILanguage language);

        void SetSystemState();

        void SetUserState();
    }
}