using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PeriodenDataModel
    {
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
