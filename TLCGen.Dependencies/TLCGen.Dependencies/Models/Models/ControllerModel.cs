using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ControllerModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        [XmlElement(ElementName = "Data")]
        public ControllerDataModel Data { get; set; }

        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<FaseCyclusModel> Fasen { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        [XmlArrayItem(ElementName = "SelectieveDetector")]
        public List<SelectieveDetectorModel> SelectieveDetectoren { get; set; }

        [XmlArrayItem(ElementName = "Ingang")]
        public List<IngangModel> Ingangen { get; set; }

        [XmlArrayItem(ElementName = "RichtingGevoeligeAanvraag")]
        public List<RichtingGevoeligeAanvraagModel> RichtingGevoeligeAanvragen { get; set; }

        [XmlArrayItem(ElementName = "RichtingGevoeligVerleng")]
        public List<RichtingGevoeligVerlengModel> RichtingGevoeligVerlengen { get; set; }

        public InterSignaalGroepModel InterSignaalGroep { get; set; } 

        [XmlArrayItem(ElementName = "GroentijdenSet")]
        public List<GroentijdenSetModel> GroentijdenSets { get; set; }

        [XmlArrayItem(ElementName = "FileIngreep")]
        public List<FileIngreepModel> FileIngrepen { get; set; }

        [XmlArrayItem(ElementName = "VAOntruimenFase")]
        public List<VAOntruimenFaseModel> VAOntruimenFasen { get; set; }

        public PelotonKoppelingenDataModel PelotonKoppelingenData { get; set; }

        public AlternatievenPerBlokModel AlternatievenPerBlokData { get; set; }

        public PeriodenDataModel PeriodenData { get; set; }

        public PTPDataModel PTPData { get; set; }

        public PrioriteitDataModel PrioData { get; set; }

        public RISDataModel RISData { get; set; }

        public ModuleMolenModel ModuleMolen { get; set; }

        [XmlArrayItem(ElementName = "ModuleMolen")]
        public List<ModuleMolenModel> MultiModuleMolens { get; set; }

        public RoBuGroverModel RoBuGrover { get; set; }

        public SignalenDataModel Signalen { get; set; }
		
		public HalfstarDataModel HalfstarData { get; set; }
		
        public StarDataModel StarData { get; set; }

		public CustomDataModel CustomData { get; set; }

        #endregion // Properties

        #region Constructor

        public ControllerModel() : base()
        {
            Data = new ControllerDataModel();
            Fasen = new List<FaseCyclusModel>();
            Detectoren = new List<DetectorModel>();
            SelectieveDetectoren = new List<SelectieveDetectorModel>();
            Ingangen = new List<IngangModel>();
            GroentijdenSets = new List<GroentijdenSetModel>();
            ModuleMolen = new ModuleMolenModel();
            MultiModuleMolens = new List<ModuleMolenModel>();
            PeriodenData = new PeriodenDataModel();
            CustomData = new CustomDataModel();
            InterSignaalGroep = new InterSignaalGroepModel();
            RichtingGevoeligeAanvragen = new List<RichtingGevoeligeAanvraagModel>();
            RichtingGevoeligVerlengen = new List<RichtingGevoeligVerlengModel>();
            FileIngrepen = new List<FileIngreepModel>();
            VAOntruimenFasen = new List<VAOntruimenFaseModel>();
            Signalen = new SignalenDataModel();
            PTPData = new PTPDataModel();
            PrioData = new PrioriteitDataModel();
            RoBuGrover = new RoBuGroverModel();
			HalfstarData = new HalfstarDataModel();
            PelotonKoppelingenData = new PelotonKoppelingenDataModel();
            RISData = new RISDataModel();
            AlternatievenPerBlokData = new AlternatievenPerBlokModel();
            StarData = new StarDataModel();
        }

        #endregion // Constructor
    }
}
