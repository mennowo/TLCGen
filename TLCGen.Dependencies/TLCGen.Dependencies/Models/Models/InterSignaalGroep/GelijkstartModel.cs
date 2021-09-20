using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GelijkstartModel : IInterSignaalGroepElement, IFormattable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int GelijkstartOntruimingstijdFaseVan { get; set; }
        public int GelijkstartOntruimingstijdFaseNaar { get; set; }
        public bool DeelConflict { get; set; }
        public Enumerations.AltijdAanUitEnum Schakelbaar { get; set; }

        #endregion // Properties
        
        #region ToString
        
        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "naarvan": return FaseNaar + FaseVan;
                case "vannaar": return FaseVan + FaseNaar;
                case "van": return FaseVan;
                case "naar": return FaseNaar;
            }

            return ToString();
        }

        #endregion // ToString
    }
}
