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
        public int GeconditioneerdePrioGrensTeVroeg { get; set; }
        public int GeconditioneerdePrioGrensTeLaat { get; set; }

        [Browsable(false)]
        [IOElement("karmelding", BitmappedItemTypeEnum.Uitgang, null, "HasAnyKAR")]
        public BitmapCoordinatenDataModel KARMeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("karog", BitmappedItemTypeEnum.Uitgang, null, "HasAnyKAR")]
        public BitmapCoordinatenDataModel KAROnderGedragBitmapData { get; set; }

        [XmlArrayItem(ElementName = "OVIngreep")]
        public List<OVIngreepModel> OVIngrepen { get; set; }
        [XmlArrayItem(ElementName = "HDIngreep")]
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<OVIngreepSignaalGroepParametersModel> OVIngreepSignaalGroepParameters { get; set; }

        [XmlArrayItem(ElementName = "WisselContact")]
        public List<WisselContactModel> WisselContacten { get; set; }

        [Browsable(false)]
        [HasDefault(false)]
        public bool HasAnyKAR => this.HasKAR();

        #endregion // Properties

        #region Public Methods

        public List<DetectorModel> GetAllDummyDetectors()
        {
            var dets = new List<DetectorModel>();

            foreach(var ov in OVIngrepen)
            {
                if(ov.Meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.KAR))
                {
                    var m = ov.Meldingen.First(x => x.Type == OVIngreepMeldingTypeEnum.KAR);
                    if (m.Inmelding) dets.Add(ov.DummyKARInmelding);
                    if (m.Uitmelding) dets.Add(ov.DummyKARUitmelding);
                }
                if (ov.Meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.VECOM))
                {
                    var m = ov.Meldingen.First(x => x.Type == OVIngreepMeldingTypeEnum.VECOM);
                    if (m.Inmelding) dets.Add(ov.DummyVecomInmelding);
                    if (m.Uitmelding) dets.Add(ov.DummyVecomUitmelding);
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
