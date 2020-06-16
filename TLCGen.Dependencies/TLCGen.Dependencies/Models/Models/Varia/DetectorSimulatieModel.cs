using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class DetectorSimulatieModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        public string RelatedName { get; set; }
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public int Stopline { get; set; }
        [HasDefault(false)]
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FCNr { get; set; }
    }
}
