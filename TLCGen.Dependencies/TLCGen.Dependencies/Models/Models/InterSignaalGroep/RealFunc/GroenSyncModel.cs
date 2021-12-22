using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    /// <summary>
    /// Represents green moment synchronisation between two signalgroups
    /// </summary>
    [Serializable]
    public class GroenSyncModel : IFormattable, IInterSignaalGroepElement
    {
        /// <summary>
        /// The origin/determining signalgroup
        /// </summary>
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseVan { get; set; }

        /// <summary>
        /// The target/receiving signalgroup
        /// </summary>
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseNaar { get; set; }

        /// <summary>
        /// The synchronisation value
        /// </summary>
        public int Waarde { get; set; }
        
        /// <summary>
        /// The synchronisation direction; this indicates whether the origin signalgroup
        /// should start before (negative value) or after the receiving signalgroup (positive value)
        /// <remarks>This is relevant mostly to determine the effect of the setting in the
        /// resulting controller application. And if the value is 0, what a later change to a higher
        /// value should mean, ie. in which direction it should affect synchronisation</remarks>
        /// </summary>
        public int Richting { get; set; }

        /// <summary>
        /// Indicates if the synchronisation can be switched on and off or not
        /// </summary>
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
