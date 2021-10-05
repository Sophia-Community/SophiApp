using SophiApp.Commons;
using SophiApp.Models;
using System;

namespace SophiApp.Helpers
{
    internal class FabricHelper
    {
        internal static TextedElement GetTextedElement(TextedElementDto dataObject, Action<TextedElement, Exception> errorHandler,
                                                            EventHandler<TextedElement> statusHandler, UILanguage language)
        {
            var type = Type.GetType($"SophiApp.Models.{dataObject.Type}");
            var element = Activator.CreateInstance(type, dataObject) as TextedElement;
            var customisation = CustomisationsHelper.GetCustomisationStatus(element.Id);
            element.Init(errorHandler, statusHandler, language, customisation);
            return element;
        }

        internal static TextedElement GetTextedElementChild(TextedChildDto dataObject, Action<TextedElement, Exception> errorHandler,
                                                                        EventHandler<TextedElement> statusHandler, UILanguage language)
        {
            var type = Type.GetType($"SophiApp.Models.{dataObject.Type}");
            var element = Activator.CreateInstance(type, dataObject) as TextedElement;
            var customisation = CustomisationsHelper.GetCustomisationStatus(element.Id);
            element.Init(errorHandler, statusHandler, language, customisation);
            return element;
        }
    }
}