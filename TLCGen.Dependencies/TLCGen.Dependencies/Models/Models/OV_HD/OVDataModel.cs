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
        public bool BlokkeerNietConflictenBijHDIngreep { get; set; }
        public bool BlokkeerNietConflictenAlleenLangzaamVerkeer { get; set; }
        public NooitAltijdAanUitEnum VerklikkenOVTellerUber { get; set; }

        [Browsable(false)]
        [IOElement("karmelding", BitmappedItemTypeEnum.Uitgang, null, "HasAnyKAR")]
        public BitmapCoordinatenDataModel KARMeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("karog", BitmappedItemTypeEnum.Uitgang, null, "HasAnyKAR")]
        public BitmapCoordinatenDataModel KAROnderGedragBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("maxwt", BitmappedItemTypeEnum.Uitgang, null, "IsOVUitgebreid")]
        public BitmapCoordinatenDataModel MaximaleWachttijdOverschredenBitmapData { get; set; }

        [XmlArrayItem(ElementName = "OVIngreep")]
        public List<OVIngreepModel> OVIngrepen { get; set; }
        [XmlArrayItem(ElementName = "HDIngreep")]
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<OVIngreepSignaalGroepParametersModel> OVIngreepSignaalGroepParameters { get; set; }
        public bool OVIngreepSignaalGroepParametersHard { get; set; }

        [Browsable(false)]
        [HasDefault(false)]
        public bool HasAnyKAR => this.HasKAR();

        [Browsable(false)]
        [HasDefault(false)]
        public bool IsOVUitgebreid => this.OVIngreepType == OVIngreepTypeEnum.Uitgebreid;

        #endregion // Properties

        #region Public Methods

        public List<DetectorModel> GetAllDummyDetectors()
        {
            var dets = new List<DetectorModel>();

            foreach(var ov in OVIngrepen)
            {
                if(ov.MeldingenData.Inmeldingen.Any(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding))
                {
                    var m = ov.MeldingenData.Inmeldingen.First(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    dets.Add(ov.DummyKARInmelding);
                }
                if (ov.MeldingenData.Uitmeldingen.Any(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding))
                {
                    var m = ov.MeldingenData.Uitmeldingen.First(x => x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    dets.Add(ov.DummyKARUitmelding);
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
            MaximaleWachttijdOverschredenBitmapData = new BitmapCoordinatenDataModel();
            OVIngrepen = new List<OVIngreepModel>();
            HDIngrepen = new List<HDIngreepModel>();
            OVIngreepSignaalGroepParameters = new List<OVIngreepSignaalGroepParametersModel>();
        }
        
        #endregion // Constructor
    }
}
