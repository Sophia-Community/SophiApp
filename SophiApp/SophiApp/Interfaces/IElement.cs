using SophiApp.Commons;
using System;
using System.ComponentModel;

namespace SophiApp.Interfaces
{
    internal interface IElement : INotifyPropertyChanged
    {
        Action<Exception> ErrorOccurred { get; set; }

        uint Id { get; }

        string Tag { get; }

        void ChangeLanguage(UILanguage language);
    }
}