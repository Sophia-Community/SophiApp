using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Customisations
{
    internal class Customisation
    {
        internal Action<bool> Action { get; }
        internal uint Id { get; }
        internal bool Parameter { get; }

        internal void Invoke() => Action.Invoke(Parameter);
        public Customisation()
        {

        }

        public Customisation(uint id, Action<bool> action, bool parameter)
        {
            Id = id;
            Action = action;
            Parameter = parameter;
        }
    }
}
