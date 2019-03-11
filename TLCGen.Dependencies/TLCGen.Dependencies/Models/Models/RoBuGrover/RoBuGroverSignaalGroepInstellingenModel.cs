using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class RoBuGroverFaseCyclusInstellingenModel
    {
        #region Properties

        [Browsable(false)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int MinGroenTijd { get; set; }
        public int MaxGroenTijd { get; set; }

        [XmlArrayItem(ElementName = "RoBuGroverFileDetector")]
        public List<RoBuGroverFileDetectorModel> FileDetectoren { get; set; }
        [XmlArrayItem(ElementName = "RoBuGroverHiaatDetector")]
        public List<RoBuGroverHiaatDetectorModel> HiaatDetectoren { get; set; }

        #endregion // Properties

        #region Constructor

        public RoBuGroverFaseCyclusInstellingenModel()
        {
            FileDetectoren = new List<RoBuGroverFileDetectorModel>();
            HiaatDetectoren = new List<RoBuGroverHiaatDetectorModel>();
        }

        #endregion // Constructor
    }
}