using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FictiefConflictModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseVan { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseNaar { get; set; }

        public int FictieveOntruimingsTijd { get; set; }
    }
}
