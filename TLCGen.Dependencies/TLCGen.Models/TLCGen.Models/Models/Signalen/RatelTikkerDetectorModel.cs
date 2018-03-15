using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("Detector")]
    public class RatelTikkerDetectorModel
    {
        public string Detector { get; set; }
    }
}
