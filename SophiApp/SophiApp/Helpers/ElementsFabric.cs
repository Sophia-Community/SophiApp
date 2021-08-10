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
        private const string CUSTOMISATION_STATE_CLASS = "SophiApp.Customisations.CustomisationState";
        private const string CUSTOMISATION_OS_CLASS = "SophiApp.Customisations.CustomisationOs";

        internal static TextedElement CreateTextedElement(JsonGuiDto dto)
        {
            var type = Type.GetType($"SophiApp.Models.{dto.Type}");
            var element = Activator.CreateInstance(type, dto) as TextedElement;

            if (element is IHasChilds childsContainer)
                childsContainer.ChildElements = dto.ChildElements.Select(child => CreateChildElement(child)).ToList();

            element.CustomisationState = GetCustomisationState(element.Id);

            return element;
        }

        private static Func<bool> GetCustomisationState(uint id)
        {
            var type = Type.GetType(CUSTOMISATION_STATE_CLASS);
            var method = type.GetMethod($"_{id}", BindingFlags.Static | BindingFlags.Public) ?? type.GetMethod("FOR_DEBUG_ONLY", BindingFlags.Static | BindingFlags.Public); //TODO: ElementsFabric - "FOR_DEBUG_ONLY"
            return Delegate.CreateDelegate(typeof(Func<bool>), method) as Func<bool>;
        }

        internal static TextedElement CreateChildElement(JsonGuiChildDto dto)
        {
            var type = Type.GetType($"SophiApp.Models.{dto.Type}");
            return Activator.CreateInstance(type, dto) as TextedElement;
        }

    }
}