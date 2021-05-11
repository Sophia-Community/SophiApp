using System;

namespace SophiApp.Actions
{
    internal class CurrentStateAction
    {
        //TODO: Implement method selection by ID

        public static bool FOR_DEBUG_ONLY() => new Random().Next(0, 2) == 0;
    }
}