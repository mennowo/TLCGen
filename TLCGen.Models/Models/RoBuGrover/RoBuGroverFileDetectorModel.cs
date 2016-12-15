using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverFileDetectorModel
    {
        public string Detector { get; set; }
        public int FileTijd { get; set; }
    }
}