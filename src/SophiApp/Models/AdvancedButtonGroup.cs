using SophiApp.Commons;
using SophiApp.Dto;
using System;

namespace SophiApp.Models
{
    internal class AdvancedButtonGroup : ButtonGroup
    {
        public AdvancedButtonGroup((TextedElementDto Dto, Action<TextedElement, Exception> ErrorHandler, EventHandler<TextedElement> StatusHandler, Func<bool> Customisation, UILanguage Language) parameters) : base(parameters)
        {
        }
    }
}