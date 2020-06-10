using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [IOElement("", BitmappedItemTypeEnum.Uitgang, "Naam")]
    public class StarProgrammaModel : IOElementModel, IHaveName, IComparable<StarProgrammaModel>
    {
        [RefersTo(TLCGenObjectTypeEnum.StarProgramma)]
        [ModelName(TLCGenObjectTypeEnum.StarProgramma)]
        [Browsable(false)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        public int Cyclustijd { get; set; }
        [XmlElement(ElementName = "Fase")]
        public List<StarProgrammaFase> Fasen { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.StarProgramma;

        #region IComparable<StarProgrammaModel>

        public int CompareTo(StarProgrammaModel other)
        {
            return string.Compare(Naam, other.Naam, StringComparison.Ordinal);
        }

        #endregion // IComparable<StarProgrammaModel>

        #region Constructor

        public StarProgrammaModel()
        {
            Fasen = new List<StarProgrammaFase>();
        }

        #endregion // Constructor
    }
}