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
        [XmlEnum("1")]
        Maandag = 1,
        [XmlEnum("2")]
        Dinsdag = 2,
        [XmlEnum("3")]
        Woensdag = 3,
        [XmlEnum("4")]
        Donderdag = 4,
        [XmlEnum("5")]
        Vrijdag = 5,
        [XmlEnum("6")]
        Zaterdag = 6,
        [XmlEnum("7")]
        Zondag = 7,
        [XmlEnum("8")]
        Werkdagen = 8,
        [XmlEnum("9")]
        Weekeind = 9,
        [XmlEnum("10")]
        AlleDagen = 10
    }
}
