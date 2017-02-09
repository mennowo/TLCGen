using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToDetector("Detector")]
    public class RoBuGroverHiaatDetectorModel
    {
        public string Detector { get; set; }
        public int HiaatTijd { get; set; }
    }
}