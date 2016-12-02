using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models.Enumerations
{
    public enum PeriodeDagCodeEnum
    {
        [XmlEnum("0")]
        Zonddag,
        [XmlEnum("1")]
        Maandag,
        [XmlEnum("2")]
        Dinsdag,
        [XmlEnum("3")]
        Woensdag,
        [XmlEnum("4")]
        Donderdag,
        [XmlEnum("5")]
        Vrijdag,
        [XmlEnum("6")]
        Zaterdag,
        [XmlEnum("7")]
        Werkdagen,
        [XmlEnum("8")]
        Weekeind,
        [XmlEnum("9")]
        KoopAvond,
        [XmlEnum("10")]
        AlleDagen
    }
}
