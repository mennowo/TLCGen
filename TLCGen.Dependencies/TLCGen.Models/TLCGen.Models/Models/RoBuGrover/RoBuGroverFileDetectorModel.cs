using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("Detector")]
    public class RoBuGroverFileDetectorModel
    {
        [Browsable(false)]
        public string Detector { get; set; }
        public int FileTijd { get; set; }
    }
}