using System;

namespace SophiApp.Customisations
{
    internal class UwpCustomisation : Customisation
    {
        public UwpCustomisation()
        {
        }

        public UwpCustomisation(string id, Action<string, bool> action, bool forAllUsers)
        {
            Id = id;
            Action = action;
            ForAllUsers = forAllUsers;
        }

        internal new Action<string, bool> Action { get; }
        internal bool ForAllUsers { get; }
        internal new string Id { get; }

        internal new void Invoke() => Action.Invoke(Id, ForAllUsers);
    }
}