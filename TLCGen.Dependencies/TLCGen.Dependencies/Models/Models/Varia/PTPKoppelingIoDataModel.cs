using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PTPKoppelingIoDataModel
    {
        // deze krijgen een naam in PtpCodeGenerator.cs
        [IOElement("ptpio", BitmappedItemTypeEnum.Uitgang, nameof(Name))]
        public BitmapCoordinatenDataModel PtpIoIsBitmapData { get; set; }

        public string Name { get; set; }
        public PTPKoppelingIoDataModel()
        {
            PtpIoIsBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
