using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ControllerRangeerDataModel
    {
        // Dit element is uitsluitend bedoeld om aan de controller model manager door te geven
        // dat data uit de oude rangeer elementen plugin is overgezet naar TLCGen
        // Over een paar versies kan dit, en de complete oude plugin, eruit.
        // (wellicht ten gunste van een andere melding in geval er oude rangeer data wordt gevonden)
        [XmlIgnore]
        public bool RangerenOvergezet { get; set; }

        public bool RangerenFasen { get; set; }
        public bool RangerenDetectoren { get; set; }
        public bool RangerenIngangen { get; set; }
        public bool RangerenUitgangen{ get; set; }
        public bool RangerenSelectieveDetectoren { get; set; }

        public List<IOElementRangeerDataModel> RangeerFasen { get; set; }
        public List<IOElementRangeerDataModel> RangeerDetectoren { get; set; }
        public List<IOElementRangeerDataModel> RangeerIngangen { get; set; }
        public List<IOElementRangeerDataModel> RangeerUitgangen { get; set; }
        public List<IOElementRangeerDataModel> RangeerSelectieveDetectoren { get; set; }

        public ControllerRangeerDataModel()
        {
            RangeerFasen = new List<IOElementRangeerDataModel>();
            RangeerDetectoren = new List<IOElementRangeerDataModel>();
            RangeerIngangen = new List<IOElementRangeerDataModel>();
            RangeerUitgangen = new List<IOElementRangeerDataModel>();
            RangeerSelectieveDetectoren = new List<IOElementRangeerDataModel>();
        }
    }
}
