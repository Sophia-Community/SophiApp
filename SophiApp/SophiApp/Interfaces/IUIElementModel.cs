using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Interfaces
{
    interface IUIElementModel
    {
        bool State { get; set; }

        bool ActualState { get; set; }

        string Description { get; set; }

        string Header { get; set; }

        int Id { get; set; }

        string Tag { get; set; }

        Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }

        Dictionary<UILanguage, string> LocalizedHeaders { get; set; }

        void SetLocalizationTo(UILanguage language);

        void ChangeActualState();
    }
}
