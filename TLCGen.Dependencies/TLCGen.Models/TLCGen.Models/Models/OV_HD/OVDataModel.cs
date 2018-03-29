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
        public bool CheckOpDSIN { get; set; }
        public int MaxWachttijdAuto { get; set; }
        public int MaxWachttijdFiets { get; set; }
        public int MaxWachttijdVoetganger { get; set; }

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

        [XmlArrayItem(ElementName = "WisselContact")]
        public List<WisselContactModel> WisselContacten { get; set; }

        #endregion // Properties

        #region Public Methods

        public List<DetectorModel> GetAllDummyDetectors()
        {
            var dets = new List<DetectorModel>();

            foreach(var ov in OVIngrepen)
            {
                if(ov.Meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.KAR))
                {
                    dets.Add(ov.DummyKARInmelding);
                    dets.Add(ov.DummyKARUitmelding);
                }
                if (ov.Meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.VECOM))
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
            WisselContacten = new List<WisselContactModel>();
        }
        
        #endregion // Constructor
    }
}
