using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class RatelTikkerModel
    {
        public RateltikkerTypeEnum Type { get; set; }
        public int NaloopTijd { get; set; }
        public string FaseCyclus { get; set; }
        public List<RatelTikkerDetectorModel> Detectoren { get; set; }

        [IOElement("rt", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        public bool ShouldSerializeNaloopTijd()
        {
            return Type == RateltikkerTypeEnum.Hoeflake;
        }

        public RatelTikkerModel()
        {
            BitmapData = new BitmapCoordinatenDataModel();
            Detectoren = new List<RatelTikkerDetectorModel>();
        }
    }
}
