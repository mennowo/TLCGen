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
        [HasDefault(false)]
        public string MeeaanvraagDetector { get; set; }
    }
}
