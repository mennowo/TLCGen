using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PeriodeModel
    {
        public string Naam { get; set; }
        public PeriodeTypeEnum Type { get; set; }
        public PeriodeDagCodeEnum DagCode { get; set; }
        [XmlIgnore]
        public TimeSpan StartTijd { get; set; }
        [XmlIgnore]
        public TimeSpan EindTijd { get; set; }
        public string GroentijdenSet { get; set; }

        // Properties for serialization
        [XmlElement("StartTijd")]
        public string SerializedStartTijd
        {
            get { return StartTijd.Hours.ToString("00") + ":" + StartTijd.Minutes.ToString("00"); }
            set
            {
                string[] parts = value.Split(':');
                if (parts.Length != 2)
                    throw new NotImplementedException();
                StartTijd = new TimeSpan(Int32.Parse(parts[0]), Int32.Parse(parts[1]), 0);
            }
        }
        [XmlElement("EindTijd")]
        public string SerializedEindTijd
        {
            get { return EindTijd.Hours.ToString("00") + ":" + EindTijd.Minutes.ToString("00"); }
            set
            {
                string[] parts = value.Split(':');
                if (parts.Length != 2)
                    throw new NotImplementedException();
                EindTijd = new TimeSpan(Int32.Parse(parts[0]), Int32.Parse(parts[1]), 0);
            }
        }

        public PeriodeModel()
        {
            StartTijd = new TimeSpan();
            EindTijd = new TimeSpan();
        }
    }
}
