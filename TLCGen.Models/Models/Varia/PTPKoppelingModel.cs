using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PTPKoppelingModel
    {
        public string TeKoppelenKruispunt { get; set; }
        public int AantalsignalenIn { get; set; }
        public int AantalsignalenUit { get; set; }
        public int PortnummerSimuatieOmgeving { get; set; }
        public int PortnummerAutomaatOmgeving { get; set; }
        public int NummerSource { get; set; }
        public int NummerDestination { get; set; }

        [IOElement("ptpok", BitmappedItemTypeEnum.Uitgang, "TeKoppelenKruispunt")]
        public BitmapCoordinatenDataModel OkBitmapData { get; set; }

        [IOElement("ptperr", BitmappedItemTypeEnum.Uitgang, "TeKoppelenKruispunt")]
        public BitmapCoordinatenDataModel ErrorBitmapData { get; set; }

        public PTPKoppelingModel()
        {
            OkBitmapData = new BitmapCoordinatenDataModel();
            ErrorBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
