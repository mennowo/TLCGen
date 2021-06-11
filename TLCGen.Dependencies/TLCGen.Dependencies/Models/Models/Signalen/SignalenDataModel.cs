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
        public bool DimmingNiveauVanuitApplicatie { get; set; }

        [IOElement("rtact", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevantAanvraag")]
        public BitmapCoordinatenDataModel RatelTikkerActiefBitmapData { get; set; }
        [IOElement("rtaltijd", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevantAltijd")]
        public BitmapCoordinatenDataModel RatelTikkerAltijdBitmapData { get; set; }
        [IOElement("rtdim", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevantDimmen")]
        public BitmapCoordinatenDataModel RatelTikkerDimmenBitmapData { get; set; }
        [IOElement("belact", BitmappedItemTypeEnum.Uitgang, "", "BellenActiefBitmapDataRelevant")]
        public BitmapCoordinatenDataModel BellenActiefBitmapData { get; set; }
        [IOElement("beldim", BitmappedItemTypeEnum.Uitgang, "", "BellenDimmenBitmapDataRelevant")]
        public BitmapCoordinatenDataModel BellenDimmenBitmapData { get; set; }
        [IOElement("perbeldim", BitmappedItemTypeEnum.Uitgang, "", "BellenDimmenBitmapDataRelevant")]
        public BitmapCoordinatenDataModel PeriodeBellenDimmenBitmapData { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodRtAanvraag { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodRtAltijd { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodRtDimmen { get; set; }

        [XmlIgnore]
        public bool ControllerHasPeriodBellenDimmen { get; set; }

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevantAanvraag => Rateltikkers.Any() && ControllerHasPeriodRtAanvraag;

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevantAltijd => Rateltikkers.Any() && ControllerHasPeriodRtAltijd;

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevantDimmen => Rateltikkers.Any() && ControllerHasPeriodRtDimmen;

        [XmlIgnore]
        public bool BellenActiefBitmapDataRelevant => WaarschuwingsGroepen.Any(x => x.Bellen);

        [XmlIgnore]
        public bool BellenDimmenBitmapDataRelevant => WaarschuwingsGroepen.Any(x => x.Bellen) && ControllerHasPeriodBellenDimmen;

        public SignalenDataModel()
        {
            WaarschuwingsGroepen = new List<WaarschuwingsGroepModel>();
            Rateltikkers = new List<RatelTikkerModel>();
            RatelTikkerActiefBitmapData = new BitmapCoordinatenDataModel();
            RatelTikkerAltijdBitmapData = new BitmapCoordinatenDataModel();
            RatelTikkerDimmenBitmapData = new BitmapCoordinatenDataModel();
            BellenActiefBitmapData = new BitmapCoordinatenDataModel();
            PeriodeBellenDimmenBitmapData = new BitmapCoordinatenDataModel();
            BellenDimmenBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
