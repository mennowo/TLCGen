using System;
using System.Collections.Generic;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RatelTikkerModel
    {
        public RateltikkerTypeEnum Type { get; set; }
        public int NaloopTijd { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public List<RatelTikkerDetectorModel> Detectoren { get; set; }

        [IOElement("rt", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        [IOElement("rtdim", BitmappedItemTypeEnum.Uitgang, "FaseCyclus", "DimmenPerUitgang")]
        public BitmapCoordinatenDataModel DimUitgangBitmapData { get; set; }

        [HasDefault(false)]
        [Browsable(false)]
        public bool DimmenPerUitgang => false;

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
