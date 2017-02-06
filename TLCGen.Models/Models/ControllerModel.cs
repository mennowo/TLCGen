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

        public PeriodenDataModel PeriodenData { get; set; }

        public PTPDataModel PTPData { get; set; }

        public OVDataModel OVData { get; set; }

        public ModuleMolenModel ModuleMolen { get; set; }

        public RoBuGroverModel RoBuGrover { get; set; }

        public SignalenDataModel Signalen { get; set; } 

        public CustomDataModel CustomData { get; set; }

        #endregion // Properties

        #region Constructor

        public ControllerModel() : base()
        {
            Data = new ControllerDataModel();
            Fasen = new List<FaseCyclusModel>();
            Detectoren = new List<DetectorModel>();
            GroentijdenSets = new List<GroentijdenSetModel>();
            ModuleMolen = new ModuleMolenModel();
            PeriodenData = new PeriodenDataModel();
            CustomData = new CustomDataModel();
            InterSignaalGroep = new InterSignaalGroepModel();
            RichtingGevoeligeAanvragen = new List<RichtingGevoeligeAanvraagModel>();
            RichtingGevoeligVerlengen = new List<RichtingGevoeligVerlengModel>();
            FileIngrepen = new List<FileIngreepModel>();
            VAOntruimenFasen = new List<VAOntruimenFaseModel>();
            Signalen = new SignalenDataModel();
            PTPData = new PTPDataModel();
            OVData = new OVDataModel();
            RoBuGrover = new RoBuGroverModel();
        }

        #endregion // Constructor
    }
}
