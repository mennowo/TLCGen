using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Helpers;

namespace TLCGen.Plugins.DynamischHiaat.Models
{
    [Serializable]
    [XmlRoot(ElementName = "DynamischHiaat")]
    public class DynamischHiaatModel
    {
        public string TypeDynamischHiaat { get; set; }

        [XmlArray(ElementName = "SignaalGroepMetDynamischHiaat")]
        public List<DynamischHiaatSignalGroupModel> SignaalGroepenMetDynamischHiaat { get; set; }

        public DynamischHiaatModel()
        {
            TypeDynamischHiaat = "IVER'18";
            SignaalGroepenMetDynamischHiaat = new List<DynamischHiaatSignalGroupModel>();
        }
    }
}
