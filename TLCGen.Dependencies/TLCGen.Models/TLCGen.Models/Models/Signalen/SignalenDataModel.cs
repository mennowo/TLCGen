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
        [IOElement("rtaltijd", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevant")]
        public BitmapCoordinatenDataModel RatelTikkerAltijdBitmapData { get; set; }
        [IOElement("rtdim", BitmappedItemTypeEnum.Uitgang, "", "RatelTikkersBitmapDataRelevant")]
        public BitmapCoordinatenDataModel RatelTikkerDimmenBitmapData { get; set; }
        [IOElement("belact", BitmappedItemTypeEnum.Uitgang, "", "WaarschuwingsGroepenBitmapDataRelevant")]
        public BitmapCoordinatenDataModel BellenActiefBitmapData { get; set; }
        [IOElement("beldim", BitmappedItemTypeEnum.Uitgang, "", "WaarschuwingsGroepenBitmapDataRelevant")]
        public BitmapCoordinatenDataModel BellenDimmenBitmapData { get; set; }

        [XmlIgnore]
        public bool RatelTikkersBitmapDataRelevant
        {
            get { return Rateltikkers.Count > 0; }
        }

        [XmlIgnore]
        public bool WaarschuwingsGroepenBitmapDataRelevant
        {
            get { return WaarschuwingsGroepen.Count > 0; }
        }

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
