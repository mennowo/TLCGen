using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    public class ControllerModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public long NextID { get; set; }

        [XmlElement(ElementName = "Data")]
        public ControllerDataModel Data { get; set; }

        [XmlArrayItem(ElementName = "Fasecyclus")]
        public List<FaseCyclusModel> Fasen { get; set; }

        #endregion // Properties

        #region Constructor

        public ControllerModel() : base()
        {
            Data = new ControllerDataModel();
            Fasen = new List<FaseCyclusModel>();
        }

        #endregion // Constructor
    }
}
