using System;

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
}