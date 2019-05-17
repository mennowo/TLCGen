using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class ControllerDataModel
    {
        #region Properties

        [HasDefault(false)]
        public string Naam { get; set; }
        [HasDefault(false)]
        public string Nummer { get; set; }
        [HasDefault(false)]
        public string Stad { get; set; }
        [HasDefault(false)]
        public string Straat1 { get; set; }
        [HasDefault(false)]
        public string Straat2 { get; set; }
        [HasDefault(false)]
        public string BitmapNaam { get; set; }
	    [HasDefault(false)]
	    public string VissimNaam { get; set; }

        [IsDocumented]
		public int Fasebewaking { get; set; }
        [IsDocumented]
        public CCOLVersieEnum CCOLVersie { get; set; }
        [IsDocumented(valueMustBe: "Geen")]
        public KWCTypeEnum KWCType { get; set; }
        public bool KWCUitgebreid { get; set; } // Note: this is not used yet, only meant for potential future functionality
        [IsDocumented(conditionProperty: "CCOLVersie", conditionPropertyValue: "CCOL8")]
        public VLOGTypeEnum VLOGType { get; set; }
        public VLOGSettingsDataModel VLOGSettings { get; set; }
        public bool VLOGInTestOmgeving { get; set; }
        public bool GenererenDuurtestCode { get; set; }
        [IsDocumented]
        public bool GarantieOntruimingsTijden { get; set; }
        [IsDocumented]
        public bool ExtraMeeverlengenInWG { get; set; }
        [IsDocumented]
        public GroentijdenTypeEnum TypeGroentijden { get; set; }
        [IsDocumented]
        public AansturingWaitsignalenEnum AansturingWaitsignalen { get; set; }

        public int WachttijdvoorspellerNietHalterenMax { get; set; }
        public int WachttijdvoorspellerNietHalterenMin { get; set; }
        public bool WachttijdvoorspellerAansturenBus { get; set; }
        public bool WachttijdvoorspellerAansturenBusHD { get; set; }

        [IsDocumented(conditionProperty: "CCOLVersie", conditionPropertyValue: "CCOL8")]
        public bool Intergroen { get; set; }

        public bool PracticeOmgeving { get; set; }
        public bool NietGebruikenBitmap { get; set; }

        public bool CCOLMulti { get; set; }
        public int CCOLMultiSlave { get; set; }

        public int HuidigeVersieMajor { get; set; }
        public int HuidigeVersieMinor { get; set; }
        public int HuidigeVersieRevision { get; set; }

        public bool AanmakenVerionSysh { get; set; }

        [Browsable(false)]
        public string TLCGenVersie { get; set; }

        [Browsable(false)]
        public List<SegmentDisplayElementModel> SegmentenDisplayBitmapData { get; set; }

        [Browsable(false)]
        public List<ModuleDisplayElementModel> ModulenDisplayBitmapData { get; set; }

        [Browsable(false)]
        public SegmentDisplayTypeEnum SegmentDisplayType { get; set; }

        public bool UitgangPerModule { get; set; }

        public bool MultiModuleReeksen { get; set; }

        public FixatieModel FixatieData { get; set; }

        [XmlIgnore]
        public bool FixatieMogelijk
        {
            get => FixatieData.FixatieMogelijk;
            set => FixatieData.FixatieMogelijk = value;
        }

        [XmlIgnore]
        public bool BijkomenTijdensFixatie
        {
            get => FixatieData.BijkomenTijdensFixatie;
            set => FixatieData.BijkomenTijdensFixatie = value;
        }


        [XmlElement(ElementName = "Versie")]
        public List<VersieModel> Versies { get; set; }

        #endregion // Properties

        #region Public Methods

        public void SetSegmentOutputs()
        {
            SegmentenDisplayBitmapData.Clear();
            switch (SegmentDisplayType)
            {
                case SegmentDisplayTypeEnum.EnkelDisplay:
                    for (int i = 1; i <= 7; ++i)
                    {
                        SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = i.ToString() });
                    }
                    break;
                case SegmentDisplayTypeEnum.DrieCijferDisplay:
                    for (int i = 1; i <= 7; ++i)
                    {
                        SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "a" + i });
                    }
                    for (int i = 1; i <= 7; ++i)
                    {
                        SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "b" + i });
                    }
                    for (int i = 1; i <= 7; ++i)
                    {
                        SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "c" + i });
                    }
                    break;
                case SegmentDisplayTypeEnum.GeenSegmenten:
                    break;
            }
        }

        #endregion // Public Methods

        #region Serialization

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if(SegmentenDisplayBitmapData?.Count == 0)
            {
                for (int i = 1; i <= 7; ++i)
                {
                    SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "segm" + i });
                }
            }
        }

        #endregion // Serialization

        #region Constructor

        public ControllerDataModel()
        {
            FixatieData = new FixatieModel();
            Versies = new List<VersieModel>();
            SegmentenDisplayBitmapData = new List<SegmentDisplayElementModel>();
            ModulenDisplayBitmapData = new List<ModuleDisplayElementModel>();
        }

        #endregion // Constructor
    }
}
