using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    internal static class ProcessHelper
    {
        internal static void Start(string uri) => Process.Start(uri);
    }
}
