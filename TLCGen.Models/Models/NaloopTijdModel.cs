using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class NaloopTijdModel
    {
        public NaloopTijdTypeEnum Type { get; set; }
        public int Waarde { get; set; }
    }
}
