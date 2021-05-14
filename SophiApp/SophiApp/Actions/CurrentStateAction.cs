using System;
using System.Threading;

namespace SophiApp.Actions
{
    internal class CurrentStateAction
    {
        //TODO: Implement method selection by ID

        public static bool FOR_DEBUG_ONLY()
        {
            Thread.Sleep(700); //TODO: For debug only !!!
            return new Random().Next(0, 2) == 1;
        }
    }
}