using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class MeeaanvraagModel : IInterSignaalGroepElement, IFormattable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public MeeaanvraagTypeEnum Type { get; set; }
        public AltijdAanUitEnum AanUit { get; set; }
        public bool TypeInstelbaarOpStraat { get; set; }
        public bool DetectieAfhankelijk { get; set; }
        public bool Uitgesteld { get; set; }
        public int UitgesteldTijdsduur { get; set; }

        [XmlArrayItem(ElementName = "MeeaanvraagDetector")]
        public List<MeeaanvraagDetectorModel> Detectoren { get; set; }

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

        public MeeaanvraagModel()
        {
            Detectoren = new List<MeeaanvraagDetectorModel>();
        }

        #endregion // Constructor
    }
}
