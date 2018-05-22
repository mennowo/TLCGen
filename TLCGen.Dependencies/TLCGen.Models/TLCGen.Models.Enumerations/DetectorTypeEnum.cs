using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DetectorTypeEnum
    {
        [Description("Koplus")]
        Kop,
        [Description("Lange lus")]
        Lang,
        [Description("Verweg lus")]
        Verweg,
        [Description("File lus")]
        File,
        [Description("Drukknop")]
        Knop,
        [Description("Drukknop binnen")]
        KnopBinnen,
        [Description("Drukknop buiten")]
        KnopBuiten,
        [Description("Radar")]
        Radar,
        [Description("Opticom ingang")]
        OpticomIngang,
        [Description("Vecom ingang (ext.)")]
        VecomDetector,
        [Description("Wissel detector")]
        WisselDetector,
        [Description("Wissel stroom kring detector")]
        WisselStroomKringDetector,
        [Description("Wissel stand detector")]
        WisselStandDetector,
        [Description("Overig")]
        Overig
    }
}
