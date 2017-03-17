using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class OVDataModel
    {
        #region Properties

        public OVIngreepTypeEnum OVIngreepType { get; set; }
        public bool DSI { get; set; }
        public bool CheckOpDSIN { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public bool HasKAR
        {
            get
            {
                return this.OVIngrepen.Count > 0 && this.OVIngrepen.Where(x => x.KAR).Any() ||
                       this.HDIngrepen.Count > 0 && this.HDIngrepen.Where(x => x.KAR).Any();
            }
        }

        [Browsable(false)]
        [IOElement("karmelding", BitmappedItemTypeEnum.Uitgang, null, "HasKAR")]
        public BitmapCoordinatenDataModel KARMeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("karog", BitmappedItemTypeEnum.Uitgang, null, "HasKAR")]
        public BitmapCoordinatenDataModel KAROnderGedragBitmapData { get; set; }

        [XmlArrayItem(ElementName = "OVIngreep")]
        public List<OVIngreepModel> OVIngrepen { get; set; }
        [XmlArrayItem(ElementName = "HDIngreep")]
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<OVIngreepSignaalGroepParametersModel> OVIngreepSignaalGroepParameters { get; set; }

        #endregion // Properties

        #region Public Methods

        public List<DetectorModel> GetAllDummyDetectors()
        {
            var dets = new List<DetectorModel>();

            foreach(var ov in OVIngrepen)
            {
                if(ov.KAR)
                {
                    dets.Add(ov.DummyKARInmelding);
                    dets.Add(ov.DummyKARUitmelding);
                }
                if (ov.Vecom)
                {
                    dets.Add(ov.DummyVecomInmelding);
                    dets.Add(ov.DummyVecomUitmelding);
                }
            }
            foreach (var hd in HDIngrepen)
            {
                if (hd.KAR)
                {
                    dets.Add(hd.DummyKARInmelding);
                    dets.Add(hd.DummyKARUitmelding);
                }
            }

                return dets;
        }

        #endregion // Public Methods

        #region Constructor

        public OVDataModel()
        {
            KARMeldingBitmapData = new BitmapCoordinatenDataModel();
            KAROnderGedragBitmapData = new BitmapCoordinatenDataModel();
            OVIngrepen = new List<OVIngreepModel>();
            HDIngrepen = new List<HDIngreepModel>();
            OVIngreepSignaalGroepParameters = new List<OVIngreepSignaalGroepParametersModel>();
        }
        
        #endregion // Constructor
    }
}
