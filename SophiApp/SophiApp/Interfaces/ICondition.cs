using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Interfaces
{
    internal interface ICondition
    {
        bool Result { get; set; }
        string Tag { get; set; }
        bool Invoke();
    }
}
