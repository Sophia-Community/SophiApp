using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Interfaces
{
    interface IRequirementTest
    {
        bool Result { get; set; }

        string ResultText { get; set; }

        void Run();
    }
}
