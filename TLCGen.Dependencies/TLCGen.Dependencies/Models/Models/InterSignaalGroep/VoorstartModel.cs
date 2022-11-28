using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class VoorstartModel : IInterSignaalGroepElement, IFormattable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int VoorstartTijd { get; set; }
        public int VoorstartOntruimingstijd { get; set; }

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
    }
}
