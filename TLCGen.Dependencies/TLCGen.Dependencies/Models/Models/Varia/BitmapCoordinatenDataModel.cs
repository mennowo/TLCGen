using System;

namespace TLCGen.Models
{
    [Serializable]
    public class BitmapCoordinatenDataModel : IOElementModel
    {
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
    }
}
