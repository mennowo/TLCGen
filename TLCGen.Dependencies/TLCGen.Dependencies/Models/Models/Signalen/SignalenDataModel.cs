using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class SignalenDataModel
    {
        [XmlArrayItem(ElementName = "WaarschuwingsGroep")]
        public List<WaarschuwingsGroepModel> WaarschuwingsGroepen { get; set; }

        [XmlArrayItem(ElementName = "Rateltikker")]
        public List<RatelTikkerModel> Rateltikkers { get; set; }

        public bool DimUitgangPerTikker { get; set; }

        [IOElement("rtact", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevant")]
        public BitmapCoordinatenDataModel RatelTikkerActiefBitmapData { get; set; }
        [IOElement("rtaltijd", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevantAltijd")]
        public BitmapCoordinatenDataModel RatelTikkerAltijdBitmapData { get; set; }
        [IOElement("rtdim", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevantDimmen")]
        public BitmapCoordinatenDataModel RatelTikkerDimmenBitmapData { get; set; }
        [IOElement("belact", BitmappedItemTypeEnum.Uitgang, "", "WaarschuwingsGroepenBitmapDataRelevant")]
        public BitmapCoordinatenDataModel BellenActiefBitmapData { get; set; }
        [IOElement("beldim", BitmappedItemTypeEnum.Uitgang, "", "WaarschuwingsGroepenBitmapDataRelevantDimmen")]
        public BitmapCoordinatenDataModel BellenDimmenBitmapData { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodRtAltijd { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodRtDimmen { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodBellenDimmen { get; set; }

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevant => Rateltikkers.Count > 0;

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevantAltijd => Rateltikkers.Count > 0 && ControllerHasPeriodRtAltijd;

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevantDimmen => Rateltikkers.Count > 0 && ControllerHasPeriodRtDimmen;

        [XmlIgnore]
        public bool WaarschuwingsGroepenBitmapDataRelevant => WaarschuwingsGroepen.Count > 0;

        [XmlIgnore]
        public bool WaarschuwingsGroepenBitmapDataRelevantDimmen => WaarschuwingsGroepen.Count > 0 && ControllerHasPeriodBellenDimmen;

        public SignalenDataModel()
        {
            WaarschuwingsGroepen = new List<WaarschuwingsGroepModel>();
            Rateltikkers = new List<RatelTikkerModel>();
            RatelTikkerActiefBitmapData = new BitmapCoordinatenDataModel();
            RatelTikkerAltijdBitmapData = new BitmapCoordinatenDataModel();
            RatelTikkerDimmenBitmapData = new BitmapCoordinatenDataModel();
            BellenActiefBitmapData = new BitmapCoordinatenDataModel();
            BellenDimmenBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
