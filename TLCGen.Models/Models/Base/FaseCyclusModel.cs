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
    public class FaseCyclusModel : IOElementModel, ITemplatable, IComparable
    {
        #region Fields

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public string Define { get; set; }
        [Browsable(false)]
        public override string Naam { get; set; }
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

        [DisplayName("OV ingreep")]
        [Description("OV ingreep")]
        public bool OVIngreep { get; set; }
        [DisplayName("HD ingreep")]
        [Description("HD ingreep")]
        public bool HDIngreep { get; set; }

        [DisplayName("Vaste aanvraag")]
        [Description("Vaste aanvraag")]
        public NooitAltijdAanUitEnum VasteAanvraag { get; set; }
        [DisplayName("Wachtgroen")]
        [Description("Wachtgroen")]
        public NooitAltijdAanUitEnum Wachtgroen { get; set; }

        [DisplayName("Meeverlengen")]
        [Description("Meeverlengen")]
        public NooitAltijdAanUitEnum Meeverlengen { get; set; }
        [DisplayName("MeeverlengenType")]
        [Description("MeeverlengenType")]
        public MeeVerlengenTypeEnum MeeverlengenType { get; set; }
        [DisplayName("MeeverlengenVerschil")]
        [Description("MeeverlengenVerschil")]
        public int MeeverlengenVerschil { get; set; }

        [DisplayName("RatelTikker")]
        [Description("RatelTikker")]
        public RateltikkerTypeEnum RatelTikkerType { get; set; }
        [DisplayName("RatelTikker naloop tijd")]
        [Description("RatelTikker naloop tijd")]
        public int RatelTikkerNaloopTijd { get; set; }
        [Browsable(false)]
        public BitmapCoordinatenDataModel RatelTikkerBitmapData { get; set; }


        public bool ShouldSerializeRatelTikkerType()
        {
            return Type == FaseTypeEnum.Voetganger;
        }
        public bool ShouldSerializeRatelTikkerNaloopTijd()
        {
            return RatelTikkerType == RateltikkerTypeEnum.Hoeflake;
        }
        public bool ShouldSerializeRatelTikkerBitmapData()
        {
            return RatelTikkerType != RateltikkerTypeEnum.Geen;
        }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

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
            foreach (ITemplatable templdp in Detectoren)
            {
                templdp.ClearAllReferences();
            }
        }


        #endregion // ITemplatable

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is FaseCyclusModel)
            {
                string s1 = (obj as FaseCyclusModel).Naam;
                string s2 = this.Naam;
                if (s1.Length < s2.Length) s1 = s1.PadLeft(s2.Length, '0');
                else if (s2.Length < s1.Length) s2 = s2.PadLeft(s1.Length, '0');

                return s2.CompareTo(s1);
            }
            return 0;
        }

        #endregion // IComparable

        #region Constructor

        public FaseCyclusModel() : base()
        {
            Detectoren = new List<DetectorModel>();
            RatelTikkerBitmapData = new BitmapCoordinatenDataModel();
        }

        #endregion // Constructor
    }
}
