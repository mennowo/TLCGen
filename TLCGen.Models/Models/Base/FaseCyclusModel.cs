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
    [RefersToSignalGroup("Naam")]
    [IOElement("", BitmappedItemTypeEnum.Fase, "Naam")]
    public class FaseCyclusModel : IOElementModel, IComparable
    {
        #region Fields

        #endregion // Fields

        #region Properties
        
        [Browsable(false)]
        public override string Naam { get; set; }
        [Browsable(false)]
        public FaseTypeEnum Type { get; set; }

        public int TFG { get; set; }
        public int TGG { get; set; }
        public int TGG_min { get; set; }
        public int TRG { get; set; }
        public int TRG_min { get; set; }
        public int TGL { get; set; }
        public int TGL_min { get; set; }
        public int Kopmax { get; set; }
        [Browsable(false)]
        public bool OVIngreep { get; set; }
        [Browsable(false)]
        public bool HDIngreep { get; set; }
        public NooitAltijdAanUitEnum VasteAanvraag { get; set; }
        public NooitAltijdAanUitEnum Wachtgroen { get; set; }
        public NooitAltijdAanUitEnum Meeverlengen { get; set; }
        public MeeVerlengenTypeEnum MeeverlengenType { get; set; }
        public int? MeeverlengenVerschil { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        #endregion // Properties

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
        }

        #endregion // Constructor
    }
}
