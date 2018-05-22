using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepInUitMeldingVoorwaardeInputOmgangMetStoringTypeEnum
    {
        [Description("Melding opvang wederzijds")]
        MeldingOpvangWederzijds,
        [Description("Melding op d1 bij storing d2")]
        MeldingD1StoringD2,
        [Description("Melding op d2 bij storing d1")]
        MeldingD2StoringD1,
        [Description("Geen melding")]
        GeenMelding,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepInUitMeldingVoorwaardeInputTypeEnum
    {
        [Description("Start detectie")]
        StartDetectie,
        [Description("Detectie op")]
        DetectieOp,
        [Description("Detectie bezet")]
        DetectieBezet,
        [Description("Start detectie bezet")]
        StartDetectieBezet,
        [Description("Einde detectie")]
        EindeDetectie,
        [Description("Einde detectie hiaat")]
        EindeDetectieHiaat
    }


    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum
    {
        [Description("KAR DSI melding")]
        KARMelding,
        [Description("Detector(en)")]
        Detector,
        [Description("VECOM via detector")]
        VecomViaDetector,
        [Description("Selectieve detector")]
        SelectieveDetector
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepInUitMeldingType
    {
        Inmelding,
        Uitmelding
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepInUitDataWisselType
    {
        Ingang,
        Detector
    }
}
