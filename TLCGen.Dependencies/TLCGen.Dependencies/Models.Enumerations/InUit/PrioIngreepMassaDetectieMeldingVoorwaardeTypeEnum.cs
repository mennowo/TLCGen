using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PrioIngreepInUitMeldingVoorwaardeTypeEnum
    {
        [Description("KAR DSI melding")]
        KARMelding,
        [Description("Detector(en)")]
        Detector,
        [Description("VECOM via detector")]
        VecomViaDetector,
        [Description("Selectieve detector")]
        SelectieveDetector,
        [Description("RIS input")]
        RISInput
    }
}
