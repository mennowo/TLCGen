using System;

namespace TLCGen.Models
{
    [Serializable]
    public class OVIngreepRitCategorieModel
    {
        [HasDefault(false)]
        public string Nummer { get; set; }
    }
}