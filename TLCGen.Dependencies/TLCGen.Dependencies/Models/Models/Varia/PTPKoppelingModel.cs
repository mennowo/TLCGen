using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
    public class PTPKoppelingModel
    {
        [RefersTo(TLCGenObjectTypeEnum.PTPKruising)]
		[ModelName(TLCGenObjectTypeEnum.PTPKruising)]
        [HasDefault(false)]
        public string TeKoppelenKruispunt { get; set; }
        public int AantalsignalenIn { get; set; }
        public int AantalsignalenUit { get; set; }
        public int AantalsignalenMultivalentIn { get; set; }
        public int AantalsignalenMultivalentUit { get; set; }
        public int PortnummerSimuatieOmgeving { get; set; }
        public int PortnummerAutomaatOmgeving { get; set; }
        public int NummerSource { get; set; }
        public int NummerDestination { get; set; }

        [IOElement("ptpok", BitmappedItemTypeEnum.Uitgang, nameof(TeKoppelenKruispunt))]
        public BitmapCoordinatenDataModel OkBitmapData { get; set; }

        [IOElement("ptperr", BitmappedItemTypeEnum.Uitgang, nameof(TeKoppelenKruispunt))]
        public BitmapCoordinatenDataModel ErrorBitmapData { get; set; }

        public PTPKoppelingModel()
        {
            OkBitmapData = new BitmapCoordinatenDataModel();
            ErrorBitmapData = new BitmapCoordinatenDataModel();
            TeKoppelenKruispunt = "";
        }
    }
}
