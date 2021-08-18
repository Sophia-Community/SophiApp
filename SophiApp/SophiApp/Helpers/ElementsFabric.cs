using SophiApp.Commons;
using SophiApp.Models;
using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class ElementsFabric
    {
        private const string CUSTOMISATION_OS_CLASS = "SophiApp.Customisations.CustomisationOs";
        private const string CUSTOMISATION_STATE_CLASS = "SophiApp.Customisations.CustomisationStatus";

        private static Func<bool> GetCustomisationStatus(uint id)
        {
            var type = Type.GetType(CUSTOMISATION_STATE_CLASS);
            var method = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public) ?? type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public); //TODO: ElementsFabric - "FOR_DEBUG_ONLY"
            return Delegate.CreateDelegate(typeof(Func<bool>), method) as Func<bool>;
        }

        internal static TextedElement CreateChildElement(TextedChildDTO dataObject,
                                                         Action<TextedElement, Exception> errorHandler,
                                                         EventHandler<TextedElement> statusHandler,
                                                         UILanguage language)
        {
            var type = Type.GetType($"SophiApp.Models.{dataObject.Type}");
            var element = Activator.CreateInstance(type, dataObject) as TextedElement;
            var customisation = GetCustomisationStatus(element.Id);
            element.Init(errorHandler, statusHandler, language, customisation);
            return element;
        }

        internal static TextedElement CreateTextedElement(TextedElementDTO dataObject,
                                                          Action<TextedElement, Exception> errorHandler,
                                                          EventHandler<TextedElement> statusHandler,
                                                          UILanguage language)
        {
            var type = Type.GetType($"SophiApp.Models.{dataObject.Type}");
            var element = Activator.CreateInstance(type, dataObject) as TextedElement;
            var customisation = GetCustomisationStatus(element.Id);
            element.Init(errorHandler, statusHandler, language, customisation);
            return element;
        }
    }
}