using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class IngangModel
    {
        [ModelName(TLCGenObjectTypeEnum.Input)]
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public IngangTypeEnum Type { get; set; }
    }
}
