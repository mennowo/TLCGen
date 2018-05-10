using System;
using System.Xml.Serialization;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenDefaultModel
    {
        public string DefaultName { get; set; }
        public string Category { get; set; }
        public string Selector1 { get; set; }
        public string Selector2 { get; set; }
        public string DataType { get; set; }
        public object Data { get; set; }

        [XmlIgnore]
        public bool Editable { get; set; }
    }
}
