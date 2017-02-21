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
        [Description("RoBuGrover in/uit")]
        public bool RoBuGrover { get; set; }
        [Description("Minimale cyclustijd")]
        public int MinimaleCyclustijd { get; set; }
        [Description("Maximale cyclustijd")]
        public int MaximaleCyclustijd { get; set; }
        [Description("Groentijd verschil")]
        public int GroentijdVerschil { get; set; }
        [Description("Methode RobuGrover")]
        public RoBuGroverMethodeEnum MethodeRoBuGrover { get; set; }
        [Description("Mate ophogen groentijd")]
        public int GroenOphoogFactor { get; set; }
        [Description("Mate verlagen groentijd")]
        public int GroenVerlaagFactor { get; set; }
        [Description("Mate verlagen bij overslag")]
        public int GroenVerlaagFactorNietPrimair { get; set; }
        [Description("Ophogen tijdens groen")]
        public bool OphogenTijdensGroen { get; set; }

        [Browsable(false)]
        [IOElement("rgv", BitmappedItemTypeEnum.Uitgang, "", "BitmapDataRelevant")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        [XmlIgnore]
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
