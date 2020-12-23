using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Interfaces
{
    internal interface IAction
    {
        Action<bool> Run { get; }        
    }
}
