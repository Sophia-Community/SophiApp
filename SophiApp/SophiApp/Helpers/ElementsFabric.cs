using SophiApp.Commons;
using SophiApp.Interfaces;
using SophiApp.Models;
using System;
using System.Linq;
using System.Reflection;

namespace SophiApp.Helpers
{
    //TODO: ElementsFabric - deprecated !!!
    internal class ElementsFabric
    {
        private const string CUSTOMISATION_OS_CLASS = "SophiApp.Customisations.CustomisationOs";
        private const string CUSTOMISATION_STATE_CLASS = "SophiApp.Customisations.CustomisationStatus";

        private static Func<bool?> SetCustomisationStatus(uint id)
        {
            var type = Type.GetType(CUSTOMISATION_STATE_CLASS);
            var method = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public) ?? type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public); //TODO: ElementsFabric - "FOR_DEBUG_ONLY"
            return Delegate.CreateDelegate(typeof(Func<bool?>), method) as Func<bool?>;
        }

        internal static TextedElement CreateTextedElement(JsonGuiDto dto)
        {
            var type = Type.GetType($"SophiApp.Models.{dto.Type}");
            var element = Activator.CreateInstance(type, dto) as TextedElement;

            if (element is IHasChilds parent)
            {
                element.Status = ElementStatus.UNCHECKED;
                parent.ChildElements = dto.ChildElements.Select(child => CreateTextedElement(child)).ToList();
            }
            else
                element.CustomisationStatus = SetCustomisationStatus(element.Id);

            return element;
        }

        internal static TextedElement CreateTextedElement(JsonGuiChildDto dto)
        {
            var type = Type.GetType($"SophiApp.Models.{dto.Type}");
            var element = Activator.CreateInstance(type, dto) as TextedElement;
            element.CustomisationStatus = SetCustomisationStatus(element.Id);
            return element;
        }
    }
}