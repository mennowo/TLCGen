using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("Detector")]
    public class NaloopDetectorModel
    {
        [HasDefault(false)]
        public string Detector { get; set; }
        public NaloopDetectorTypeEnum Type { get; set; }
    }
}
