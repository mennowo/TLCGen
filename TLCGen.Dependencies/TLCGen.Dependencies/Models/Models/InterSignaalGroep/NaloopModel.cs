using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class NaloopModel : IInterSignaalGroepElement, IFormattable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public NaloopTypeEnum Type { get; set; }
        public bool VasteNaloop { get; set; }
        public bool InrijdenTijdensGroen { get; set; }
        public bool DetectieAfhankelijk { get; set; }
        public int? MaximaleVoorstart { get; set; }
        public int MaxUitverlengenVolgrichting { get; set; }
        public bool LosseRealisatieVoedendeRichting { get; set; }
        public bool LosseRealisatieVoorwaardeGeenAanvraagNaloop { get;set; }

        [XmlArrayItem(ElementName = "NaloopDetector")]
        public List<NaloopDetectorModel> Detectoren { get; set; }

        [XmlArrayItem(ElementName = "NaloopTijden")]
        public List<NaloopTijdModel> Tijden { get; set; }

        #endregion // Properties

        #region ToString

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "vannaar": return FaseVan + FaseNaar;
                case "naarvan": return FaseNaar + FaseVan;
                case "van": return FaseVan;
                case "naar": return FaseNaar;
            }

            return ToString();
        }

        #endregion // ToString

        #region Constructor

        public NaloopModel()
        {
            Detectoren = new List<NaloopDetectorModel>();
            Tijden = new List<NaloopTijdModel>();
            VasteNaloop = true;
        }

        #endregion // Constructor
    }
}
