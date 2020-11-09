using SophiAppCE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Models
{
    public class BaseModel
    {
        public string Id { get; set; }
        public ControlsType Type { get; set; }
        public Tags Tag { get; set; }
    }
}
