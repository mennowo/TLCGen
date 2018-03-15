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
    [RefersTo("FaseVan", "FaseNaar")]
    public class MeeaanvraagModel : IInterSignaalGroepElement
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string FaseVan { get; set; }
        public string FaseNaar { get; set; }
        public MeeaanvraagTypeEnum Type { get; set; }
        public bool TypeInstelbaarOpStraat { get; set; }
        public bool DetectieAfhankelijk { get; set; }
        public bool Uitgesteld { get; set; }
        public int UitgesteldTijdsduur { get; set; }

        [XmlArrayItem(ElementName = "MeeaanvraagDetector")]
        public List<MeeaanvraagDetectorModel> Detectoren { get; set; }

        #endregion // Properties

        #region Constructor

        public MeeaanvraagModel()
        {
            Detectoren = new List<MeeaanvraagDetectorModel>();
        }

        #endregion // Constructor
    }
}
