using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenTemplatesModel
    {
        [XmlArrayItem(ElementName = "FasenTemplate")]
        public List<TLCGenTemplateModel<FaseCyclusModel>> FasenTemplates { get; set; }
        [XmlArrayItem(ElementName = "DetectorenTemplate")]
        public List<TLCGenTemplateModel<DetectorModel>> DetectorenTemplates { get; set; }
        [XmlArrayItem(ElementName = "PeriodeTemplate")]
        public List<TLCGenTemplateModel<PeriodeModel>> PeriodenTemplates { get; set; }

        public TLCGenTemplatesModel()
        {
            FasenTemplates = new List<TLCGenTemplateModel<FaseCyclusModel>>();
            DetectorenTemplates = new List<TLCGenTemplateModel<DetectorModel>>();
            PeriodenTemplates = new List<TLCGenTemplateModel<PeriodeModel>>();
        }
    }
}
