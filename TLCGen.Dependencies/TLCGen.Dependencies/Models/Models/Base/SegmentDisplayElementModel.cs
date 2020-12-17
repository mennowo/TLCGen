using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class SegmentDisplayElementModel
    {
        [IOElement("segm", BitmappedItemTypeEnum.Uitgang, "Naam")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }
        private string _Naam;

        [HasDefault(false)]
        public string Naam
        {
            get => _Naam;
            set
            {
                _Naam = value;
                BitmapData.Naam = value;
            }
        }

        public SegmentDisplayElementModel()
        {
            BitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
