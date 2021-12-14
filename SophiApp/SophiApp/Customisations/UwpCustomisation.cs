using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Customisations
{
    internal class UwpCustomisation : Customisation
    {
        internal new Action<string, bool> Action { get; }
        internal new string Id { get; }
        internal bool ForAllUsers { get; }

        internal new void Invoke() => Action.Invoke(Id, ForAllUsers);

        public UwpCustomisation()
        {

        }

        public UwpCustomisation(string id, Action<string, bool> action, bool forAllUsers)
        {
            Id = id;
            Action = action;
            ForAllUsers = forAllUsers;
        }

    }
}
