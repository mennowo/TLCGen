using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.AFM.Models
{

    [Serializable]
    [XmlRoot(ElementName = "AFMData")]
    public class AFMDataModel
    {
        public bool AFMToepassen { get; set; }

		[IOElement("mlact", BitmappedItemTypeEnum.Uitgang, conditionprop:"AFMToepassen")]
        public BitmapCoordinatenDataModel AFMLevenBitmapCoordinaten { get; set; }

        [XmlArray(ElementName = "AFMFaseCyclusData")]
        public List<AFMFaseCyclusDataModel> AFMFasen { get; set; }

        public AFMDataModel()
        {
            AFMFasen = new List<AFMFaseCyclusDataModel>();
            AFMLevenBitmapCoordinaten = new BitmapCoordinatenDataModel();
        }
    }
}
