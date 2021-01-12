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
        [Description("RIS")]
        RISVoorwaarde,
        //[Description("RIS vracht")]
        //VrachtRIS,
        //[Description("RIS peloton fiets")]
        //FietsRISPeloton,
        [Description("Fiets prioriteit")]
        FietsMassaPeloton,
        //[Description("RIS peloton auto")]
        //AutoRISPeloton,
        [Description("Massa peloton auto")]
        AutoMassaPeloton,
    }
}
