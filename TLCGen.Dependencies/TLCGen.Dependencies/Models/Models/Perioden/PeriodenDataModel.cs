using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PeriodenDataModel
    {
        public bool GebruikPeriodenNamen { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.GroenTijdenSet)]
        public string DefaultPeriodeGroentijdenSet { get; set; }
        public string DefaultPeriodeNaam { get; set; }

        [IOElement("perdef", BitmappedItemTypeEnum.Uitgang)]
        public BitmapCoordinatenDataModel DefaultPeriodeBitmapData { get; set; }

        [XmlArrayItem(ElementName = "Periode")]
        public List<PeriodeModel> Perioden { get; set; }

        public PeriodenDataModel()
        {
            Perioden = new List<PeriodeModel>();
            DefaultPeriodeBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
