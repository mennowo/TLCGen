using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class DetectorSimulatieModel
    {
        [RefersTo]
        public string RelatedName { get; set; }
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public int Stopline { get; set; }
        [HasDefault(false)]
        public string FCNr { get; set; }
    }
}
