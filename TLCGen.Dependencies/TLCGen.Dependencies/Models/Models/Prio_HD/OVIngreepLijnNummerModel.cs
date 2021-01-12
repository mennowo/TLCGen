using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class OVIngreepLijnNummerModel
    {
        [HasDefault(false)]
        public string Nummer { get; set; }

        [HasDefault(false)]
        public string RitCategorie { get; set; }
    }

    [Serializable]
    public class OVIngreepPeriodeModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Periode)]
        [HasDefault(false)]
        public string Periode { get; set; }
    }
}