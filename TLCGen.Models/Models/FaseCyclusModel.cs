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
        [DisplayName("Type fase")]
        [Description("Type fase")]
        public FaseTypeEnum Type { get; set; }
        [DisplayName("Vastgroen tijd")]
        [Description("Vastgroen tijd")]
        public int TFG { get; set; }
        [DisplayName("Garantie groen tijd")]
        [Description("Garantie groen tijd")]
        public int TGG { get; set; }
        [DisplayName("Minimum garantie groen tijd")]
        [Description("Minimum garantie groen tijd")]
        public int TGG_min { get; set; }
        [DisplayName("Garantie rood tijd")]
        [Description("Garantie rood tijd")]
        public int TRG { get; set; }
        [DisplayName("Minimum garantie rood tijd")]
        [Description("Minimum garantie rood tijd")]
        public int TRG_min { get; set; }
        [DisplayName("Geel tijd")]
        [Description("Geel tijd")]
        public int TGL { get; set; }
        [DisplayName("Minimum geel tijd")]
        [Description("Minimum geel tijd")]
        public int TGL_min { get; set; }
        [DisplayName("Koplus maximum")]
        [Description("Koplus maximum")]
        public int Kopmax { get; set; }

        [DisplayName("Vaste aanvraag")]
        [Description("Vaste aanvraag")]
        public NooitAltijdAanUitEnum VasteAanvraag { get; set; }
        [DisplayName("Wachtgroen")]
        [Description("Wachtgroen")]
        public NooitAltijdAanUitEnum Wachtgroen { get; set; }
        [DisplayName("Meeverlengen")]
        [Description("Meeverlengen")]
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
