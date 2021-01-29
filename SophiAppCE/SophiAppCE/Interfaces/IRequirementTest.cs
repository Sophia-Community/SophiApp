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

        string Name { get; set; }

        string ErrorDescription { get; set; }
        
        string Url { get; set; }

        void Run();
    }
}
