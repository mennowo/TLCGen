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
    public class PrioriteitDataModel
    {
        #region Properties

        public PrioIngreepTypeEnum PrioIngreepType { get; set; }
        public bool CheckOpDSIN { get; set; }
        public int MaxWachttijdAuto { get; set; }
        public int MaxWachttijdFiets { get; set; }
        public int MaxWachttijdVoetganger { get; set; }
        public int GeconditioneerdePrioGrensTeVroeg { get; set; }
        public int GeconditioneerdePrioGrensTeLaat { get; set; }
        public bool BlokkeerNietConflictenBijHDIngreep { get; set; }
        public bool BlokkeerNietConflictenAlleenLangzaamVerkeer { get; set; }
        public NooitAltijdAanUitEnum VerklikkenPrioTellerUber { get; set; }
        public bool VerlaagHogeSignaalGroepNummers { get; set; }

        [Browsable(false)]
        [IOElement("karmelding", BitmappedItemTypeEnum.Uitgang, null, "HasAnyKAR")]
        public BitmapCoordinatenDataModel KARMeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("karog", BitmappedItemTypeEnum.Uitgang, null, "HasAnyKAR")]
        public BitmapCoordinatenDataModel KAROnderGedragBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("maxwt", BitmappedItemTypeEnum.Uitgang, null, "IsOVUitgebreid")]
        public BitmapCoordinatenDataModel MaximaleWachttijdOverschredenBitmapData { get; set; }

        [XmlArrayItem(ElementName = "PrioIngreep")]
        public List<PrioIngreepModel> PrioIngrepen { get; set; }
        [XmlArrayItem(ElementName = "HDIngreep")]
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<PrioIngreepSignaalGroepParametersModel> PrioIngreepSignaalGroepParameters { get; set; }
        public bool PrioIngreepSignaalGroepParametersHard { get; set; }

        [Browsable(false)]
        [HasDefault(false)]
        public bool HasAnyKAR => this.HasKAR();

        #endregion // Properties

        #region Public Methods

        public List<DetectorModel> GetAllDummyDetectors()
        {
            var dets = new List<DetectorModel>();

            foreach(var prio in PrioIngrepen)
            {
                if(prio.MeldingenData.Inmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding))
                {
                    var m = prio.MeldingenData.Inmeldingen.First(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    dets.Add(prio.DummyKARInmelding);
                }
                if (prio.MeldingenData.Uitmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding))
                {
                    var m = prio.MeldingenData.Uitmeldingen.First(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    dets.Add(prio.DummyKARUitmelding);
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

        public PrioriteitDataModel()
        {
            KARMeldingBitmapData = new BitmapCoordinatenDataModel();
            KAROnderGedragBitmapData = new BitmapCoordinatenDataModel();
            MaximaleWachttijdOverschredenBitmapData = new BitmapCoordinatenDataModel();
            PrioIngrepen = new List<PrioIngreepModel>();
            HDIngrepen = new List<HDIngreepModel>();
            PrioIngreepSignaalGroepParameters = new List<PrioIngreepSignaalGroepParametersModel>();
        }
        
        #endregion // Constructor
    }
}
