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
        Zondag = 1,
        [XmlEnum("2")]
        Maandag = 2,
        [XmlEnum("3")]
        Dinsdag = 3,
        [XmlEnum("4")]
        Woensdag = 4,
        [XmlEnum("5")]
        Donderdag = 5,
        [XmlEnum("6")]
        Vrijdag = 6,
        [XmlEnum("7")]
        Zaterdag = 7,
        [XmlEnum("8")]
        Werkdagen = 8,
        [XmlEnum("9")]
        Weekeind = 9,
        [XmlEnum("10")]
        AlleDagen = 10
    }
}
