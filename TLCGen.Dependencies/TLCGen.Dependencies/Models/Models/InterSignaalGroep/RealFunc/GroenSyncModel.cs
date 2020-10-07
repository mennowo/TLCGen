using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GroenSyncModel : IFormattable
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseVan { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseNaar { get; set; }

        public int Waarde { get; set; }

        public AltijdAanUitEnum AanUit { get; set; }
        
        public override string ToString()
        {
            return FaseVan + FaseNaar;
        }

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
    }
}
