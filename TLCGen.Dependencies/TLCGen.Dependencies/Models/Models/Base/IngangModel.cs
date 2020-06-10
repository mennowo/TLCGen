using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [IOElement("", BitmappedItemTypeEnum.Ingang, "Naam")]
    public class IngangModel : IOElementModel, IComparable, IHaveName
    {
        [RefersTo(TLCGenObjectTypeEnum.Input)]
        [ModelName(TLCGenObjectTypeEnum.Input)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        [HasDefault(false)]
        public string Omschrijving { get; set; }
        public IngangTypeEnum Type { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.Input;

        public int CompareTo(object obj)
        {
            return Naam.CompareTo(((IngangModel)obj).Naam);
        }
    }
}
