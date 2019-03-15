using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class ModuleFaseCyclusModel : IComparable<ModuleFaseCyclusModel>
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        [XmlArrayItem(ElementName = "Fase")]
        public List<ModuleFaseCyclusAlternatiefModel> Alternatieven { get; set; }

        #endregion // Properties

        public int CompareTo(ModuleFaseCyclusModel other)
        {
            return FaseCyclus.CompareTo(other.FaseCyclus);
        }

        #region Constructor

        public ModuleFaseCyclusModel()
        {
            Alternatieven = new List<ModuleFaseCyclusAlternatiefModel>();
        }

        #endregion // Constructor

    }
}
