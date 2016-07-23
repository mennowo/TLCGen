using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Enumerations;

namespace TLCGen.Models
{
    public class FaseCyclusModel : ModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Naam { get; set; }
        public string Define { get; set; }
        public FaseTypeEnum Type { get; set; }
        public int TFG { get; set; }
        public int TGG { get; set; }
        public int TGG_min { get; set; }
        public int TRG { get; set; }
        public int TRG_min { get; set; }
        public int TGL { get; set; }
        public int TGL_min { get; set; }

        public bool VasteAanvraag { get; set; }
        public bool Meeverlengen { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        [XmlArrayItem(ElementName = "Conflict")]
        public List<ConflictModel> Conflicten { get; set; }

        #endregion // Properties

        #region Constructor

        public FaseCyclusModel() : base()
        {
            Detectoren = new List<DetectorModel>();
            Conflicten = new List<ConflictModel>();
        }

        #endregion // Constructor
    }
}
