using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverModel
    {
        public bool RoBuGrover { get; set; }
        public int MinimaleCyclustijd { get; set; }
        public int MaximaleCyclustijd { get; set; }
        public int GroentijdVerschil { get; set; }
        public RoBuGroverMethodeEnum MethodeRoBuGrover { get; set; }
        public int GroenOphoogFactor { get; set; }
        public int GroenVerlaagFactor { get; set; }
        public int GroenVerlaagFactorNietPrimair { get; set; }
        public bool OphogenTijdensGroen { get; set; }

        [Browsable(false)]
        [IOElement("rgv", BitmappedItemTypeEnum.Uitgang, "", "BitmapDataRelevant")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        [HasDefault(false)]
        public bool BitmapDataRelevant
        {
            get { return ConflictGroepen.Count > 0; }
        }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "RoBuGroverConflictGroep")]
        public List<RoBuGroverConflictGroepModel> ConflictGroepen { get; set; }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "RoBuGroverSignaalGroepInstelling")]
        public List<RoBuGroverFaseCyclusInstellingenModel> SignaalGroepInstellingen { get; set; }

        public RoBuGroverModel()
        {
            ConflictGroepen = new List<RoBuGroverConflictGroepModel>();
            SignaalGroepInstellingen = new List<RoBuGroverFaseCyclusInstellingenModel>();
            BitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
