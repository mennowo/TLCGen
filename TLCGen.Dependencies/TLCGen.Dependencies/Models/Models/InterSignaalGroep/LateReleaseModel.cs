using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class LateReleaseModel : IInterSignaalGroepElement, IFormattable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseVan { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int LateReleaseTijd { get; set; }
        public int LateReleaseOntruimingstijd { get; set; }

        #endregion // Properties
        
        #region ToString
        
        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "naarvan": return FaseNaar + FaseVan;
                case "van": return FaseVan;
                case "naar": return FaseNaar;
            }

            return ToString();
        }

        #endregion // ToString
    }
}
