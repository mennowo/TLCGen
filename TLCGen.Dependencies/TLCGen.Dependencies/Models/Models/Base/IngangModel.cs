using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [RefersTo(TLCGenObjectTypeEnum.Input, "Naam")]
    [Serializable]
    [IOElement("", BitmappedItemTypeEnum.Ingang, "Naam")]
    public class IngangModel : IOElementModel, IComparable, IHaveName
    {
        [ModelName(TLCGenObjectTypeEnum.Input)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        [HasDefault(false)]
        public string Omschrijving { get; set; }
        public IngangTypeEnum Type { get; set; }

        public int CompareTo(object obj)
        {
            return Naam.CompareTo(((IngangModel)obj).Naam);
        }
    }
}
