using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Models
{
    class SwitchModel : IUIElementModel
    {
        public string Description { get; set; }
        public string Header { get; set; }
        public int Id { get; set; }
        public string Tag { get; set; }
        public Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }
        public Dictionary<UILanguage, string> LocalizedHeaders { get; set; }
        
        public SwitchModel(JsonDTO json)
        {
            LocalizedDescriptions = UILocalization.GetLocalizedDescriptions(json);
            LocalizedHeaders = UILocalization.GetLocalizedHeaders(json);
            Id = json.Id;
            Tag = json.Tag;
        }

        public void SetLocalizationTo(UILanguage language)
        {
            Header = LocalizedHeaders[language];
            Description = LocalizedDescriptions[language];
        }
    }
}
