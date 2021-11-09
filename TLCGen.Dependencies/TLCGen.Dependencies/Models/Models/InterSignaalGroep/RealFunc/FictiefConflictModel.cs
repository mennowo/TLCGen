using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FictiefConflictModel : IFormattable
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseVan { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseNaar { get; set; }

        public int FictieveOntruimingsTijd { get; set; }

        public override string ToString()
        {
            return FaseVan + FaseNaar;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "naarvan": return FaseNaar + FaseVan;
                case "vannaar": return FaseVan + FaseNaar;
                case "van": return FaseNaar;
                case "naar": return FaseVan;
            }

            return ToString();
        }
    }
}
