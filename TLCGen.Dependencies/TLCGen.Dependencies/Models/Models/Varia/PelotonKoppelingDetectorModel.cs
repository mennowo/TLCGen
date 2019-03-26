using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PelotonKoppelingDetectorModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        public string DetectorNaam { get; set; }
        public int KoppelSignaal { get; set; }
    }
}
