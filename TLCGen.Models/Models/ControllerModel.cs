using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ControllerModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        [XmlIgnore]
        public long NextID { get; set; }

        [XmlElement(ElementName = "Data")]
        public ControllerDataModel Data { get; set; }

        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<FaseCyclusModel> Fasen { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        [XmlArrayItem(ElementName = "MaxGroentijdenSet")]
        public List<MaxGroentijdenSetModel> MaxGroentijdenSets { get; set; }

        public ModuleMolenModel ModuleMolen { get; set; }

        public CustomDataModel CustomData { get; set; }

        #endregion // Properties

        #region Constructor

        public ControllerModel() : base()
        {
            Data = new ControllerDataModel();
            Fasen = new List<FaseCyclusModel>();
            Detectoren = new List<DetectorModel>();
            MaxGroentijdenSets = new List<MaxGroentijdenSetModel>();
            ModuleMolen = new ModuleMolenModel();
            CustomData = new CustomDataModel();
        }

        #endregion // Constructor
    }
}
