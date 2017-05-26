using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToDetector("Detector")]
    public class FileIngreepDetectorModel
    {
        [HasDefault(false)]
        public string Detector { get; set; }

        public int BezetTijd { get; set; }
        public int RijTijd { get; set; }
        public int AfvalVertraging { get; set; }
    }
}
