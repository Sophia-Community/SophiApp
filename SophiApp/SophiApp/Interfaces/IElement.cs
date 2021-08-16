using SophiApp.Commons;
using System.ComponentModel;

namespace SophiApp.Interfaces
{
    internal interface IElement : INotifyPropertyChanged
    {
        uint Id { get; }

        string Tag { get; }

        void ChangeLanguage(UILanguage language);
    }
}