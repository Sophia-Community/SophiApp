using System;

namespace SophiApp.Customisations
{
    internal class Customisation
    {
        public Customisation()
        {
        }

        public Customisation(uint id, Action<bool> action, bool parameter)
        {
            Id = id;
            Action = action;
            Parameter = parameter;
        }

        internal Action<bool> Action { get; }
        internal uint Id { get; }
        internal bool Parameter { get; }

        internal void Invoke() => Action.Invoke(Parameter);
    }
}