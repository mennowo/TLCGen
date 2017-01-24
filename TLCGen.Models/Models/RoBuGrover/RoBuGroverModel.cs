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
    public class RoBuGroverModel
    {
        public int MaximaleCyclustijd { get; set; }
        public RoBuGroverMethodeEnum MethodeRoBuGrover { get; set; }
        public bool OphogenTijdensGroen { get; set; }
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        [XmlArrayItem(ElementName = "RoBuGroverConflictGroep")]
        public List<RoBuGroverConflictGroepModel> ConflictGroepen { get; set; }
        [XmlArrayItem(ElementName = "RoBuGroverSignaalGroepInstelling")]
        public List<RoBuGroverSignaalGroepInstellingenModel> SignaalGroepInstellingen { get; set; }

        public RoBuGroverModel()
        {
            ConflictGroepen = new List<RoBuGroverConflictGroepModel>();
            SignaalGroepInstellingen = new List<RoBuGroverSignaalGroepInstellingenModel>();
            BitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
