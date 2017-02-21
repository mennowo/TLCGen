using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;
using TLCGen.Models.Settings;

namespace TLCGen.Models
{
    [Serializable]
    public class ControllerDataModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Naam { get; set; }
        public string Stad { get; set; }
        public string Straat1 { get; set; }
        public string Straat2 { get; set; }
        public string BitmapNaam { get; set; }

        public List<SegmentDisplayElementModel> SegmentenDisplayBitmapData { get; set; }

        public CCOLVersieEnum CCOLVersie { get; set; }
        public KWCTypeEnum KWCType { get; set; }
        public VLOGTypeEnum VLOGType { get; set; }
        public bool VLOGInTestOmgeving { get; set; }
        public TLCGenVersieEnum TLCGenVersie { get; set; }

        public bool GarantieOntruimingsTijden { get; set; }
        public bool ExtraMeeverlengenInWG { get; set; }
        public GroentijdenTypeEnum TypeGroentijden { get; set; }
        public AansturingWaitsignalenEnum AansturingWaitsignalen { get; set; }
        public OVIngreepTypeEnum OVIngreep { get; set; }
        public bool DSI { get; set; }

        private SegmentDisplayTypeEnum _SegmentDisplayType;
        public SegmentDisplayTypeEnum SegmentDisplayType
        {
            get { return _SegmentDisplayType; }
            set
            {
                _SegmentDisplayType = value;
            }
        }

        public FixatieModel FixatieData { get; set; }

        public List<VersieModel> Versies { get; set; }

        #endregion // Properties

        #region Public Methods

        public void SetSegmentOutputs()
        {
            SegmentenDisplayBitmapData.Clear();
            switch (_SegmentDisplayType)
            {
                case SegmentDisplayTypeEnum.EnkelDisplay:
                    if (SegmentenDisplayBitmapData.Count == 0)
                    {
                        for (int i = 1; i <= 7; ++i)
                        {
                            SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "segm" + i });
                        }
                    }
                    break;
                case SegmentDisplayTypeEnum.DrieCijferDisplay:
                    if (SegmentenDisplayBitmapData.Count == 0)
                    {
                        for (int i = 1; i <= 7; ++i)
                        {
                            SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "segma" + i });
                        }
                        for (int i = 1; i <= 7; ++i)
                        {
                            SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "segmb" + i });
                        }
                        for (int i = 1; i <= 7; ++i)
                        {
                            SegmentenDisplayBitmapData.Add(new SegmentDisplayElementModel() { Naam = "segmc" + i });
                        }
                    }
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
        }

        #endregion // Constructor
    }
}
