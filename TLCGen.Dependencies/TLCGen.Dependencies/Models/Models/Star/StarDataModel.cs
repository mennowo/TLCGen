using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class StarDataModel
    {
        public bool ToepassenStar { get; set; }
        [XmlElement(ElementName = "Programma")]
        public List<StarProgrammaModel> Programmas { get; set; }
        public StarPeriodeDataModel DefaultProgramma { get; set; }
        public List<StarPeriodeDataModel> PeriodenData { get; set; }

        public StarDataModel()
        {
            Programmas = new List<StarProgrammaModel>();
            PeriodenData = new List<StarPeriodeDataModel>();
        }
    }

    [Serializable]
    public class StarPeriodeDataModel
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Periode)]
        [HasDefault(false)]
        public string Periode { get; set; }
        [HasDefault(false)]
        [RefersTo(TLCGenObjectTypeEnum.StarProgramma)]
        public string StarProgramma { get; set; }
        
        #endregion // Properties
    }

    [Serializable]
    [IOElement("", BitmappedItemTypeEnum.Uitgang, "Naam")]
    public class StarProgrammaModel : IOElementModel, IHaveName, IComparable<StarProgrammaModel>
    {
        [ModelName(TLCGenObjectTypeEnum.StarProgramma)]
        [Browsable(false)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        public int Cyclustijd { get; set; }
        [XmlElement(ElementName = "Fase")]
        public List<StarProgrammaFase> Fasen { get; set; }

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

    [RefersTo(TLCGenObjectTypeEnum.Fase, nameof(FaseCyclus))]
    public class StarProgrammaFase
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int Start1 { get; set; }
        public int Eind1 { get; set; }
        public int? Start2 { get; set; }
        public int? Eind2 { get; set; }

        #endregion // Properties
    }
}
