using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public BitmapCoordinatenDataModel OkBitmapData { get; set; }
        public BitmapCoordinatenDataModel ErrorBitmapData { get; set; }

        public PTPKoppelingModel()
        {
            OkBitmapData = new BitmapCoordinatenDataModel();
            ErrorBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
