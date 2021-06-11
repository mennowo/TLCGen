using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    public enum IOElementTypeEnum
    {
        FaseCyclus,
        Detector,
        Output,
        Input,
        SelectiveDetector
    }

    [Serializable]
    public abstract class IOElementModel
    {
        [HasDefault(false)]
        public abstract string Naam { get; set; }

        public abstract bool Dummy { get; set; }

        [XmlIgnore]
        [HasDefault(false)]
        public int RangeerIndex { get; set; }
        
        [XmlIgnore]
        [HasDefault(false)]
        public int RangeerIndex2 { get; set; }

        [HasDefault(false)]
        public IOElementTypeEnum ElementType { get; set; }

        [XmlIgnore]
        [HasDefault(false)]
        public bool Multivalent { get; set; }

        [Browsable(false)]
        [XmlArrayItem(ElementName = "Coordinaat")]
        public List<BitmapCoordinaatModel> BitmapCoordinaten { get; set; }

        public override string ToString()
        {
            return Naam;
        }

        public IOElementModel()
        {
            BitmapCoordinaten = new List<BitmapCoordinaatModel>();
        }
    }
}
