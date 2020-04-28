using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GroenSyncModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseVan { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseNaar { get; set; }

        public int Waarde { get; set; }
    }
}
