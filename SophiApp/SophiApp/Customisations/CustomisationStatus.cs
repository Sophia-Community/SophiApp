using System;

namespace SophiApp.Customisations
{
    public class CustomisationStatus
    {
        //TODO: CustomisationState - Method placeholder.
        public static bool? FOR_DEBUG_ONLY() => new Random().Next(101) <= 50;
    }
}