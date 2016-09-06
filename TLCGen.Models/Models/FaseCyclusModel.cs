using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Interfaces;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FaseCyclusModel : IOElementModel, ITemplatable
    {
        #region Fields

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public string Define { get; set; }
        [Browsable(false)]
        public string Naam { get; set; }
        public FaseTypeEnum Type { get; set; }
        public int TFG { get; set; }
        public int TGG { get; set; }
        public int TGG_min { get; set; }
        public int TRG { get; set; }
        public int TRG_min { get; set; }
        public int TGL { get; set; }
        public int TGL_min { get; set; }
        public int Kopmax { get; set; }

        public NooitAltijdAanUitEnum VasteAanvraag { get; set; }
        public NooitAltijdAanUitEnum Wachtgroen { get; set; }
        public NooitAltijdAanUitEnum Meeverlengen { get; set; }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "Conflict")]
        public List<ConflictModel> Conflicten { get; set; }

        #endregion // Properties

        #region ITemplatable

        public string GetIdentifyingName()
        {
            return Naam;
        }

        public void SetAllIdentifyingNames(string search, string replace)
        {
            Naam = Naam.Replace(search, replace);
            Define = Define.Replace(search, replace);
            foreach(ITemplatable templdp in Detectoren)
            {
                templdp.SetAllIdentifyingNames(search, replace);
            }
        }

        public void ClearAllReferences()
        {
            BitmapCoordinaten.Clear();
            Conflicten.Clear();
            foreach (ITemplatable templdp in Detectoren)
            {
                templdp.ClearAllReferences();
            }
        }

        #endregion // ITemplatable

        #region Constructor

        public FaseCyclusModel() : base()
        {
            Detectoren = new List<DetectorModel>();
            Conflicten = new List<ConflictModel>();
        }

        #endregion // Constructor
    }
}
