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
        [IOElement("maxwt", BitmappedItemTypeEnum.Uitgang, null, "HasPrio")]
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

        [Browsable(false)]
        [HasDefault(false)]
        public bool HasPrio => this.PrioIngreepType != PrioIngreepTypeEnum.Geen;

        #endregion // Properties

        #region Public Methods

        public List<DetectorModel> GetAllDummyDetectors()
        {
            var detsIn = PrioIngrepen.SelectMany(x => x.MeldingenData.Inmeldingen.Where(x2 => x2.DummyKARMelding != null).Select(x2 => x2.DummyKARMelding));
            var detsUit = PrioIngrepen.SelectMany(x => x.MeldingenData.Uitmeldingen.Where(x2 => x2.DummyKARMelding != null).Select(x2 => x2.DummyKARMelding));
            var dets = detsIn.Concat(detsUit).ToList();
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
