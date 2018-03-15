using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("MeeaanvraagDetector")]
    public class MeeaanvraagDetectorModel
    {
        public string MeeaanvraagDetector { get; set; }
    }
}
