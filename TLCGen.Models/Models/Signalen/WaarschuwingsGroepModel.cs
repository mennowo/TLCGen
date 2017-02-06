using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class WaarschuwingsGroepModel
    {
        public string Naam { get; set; }
        public bool Lichten { get; set; }
        public bool Bellen { get; set; }
        public string FaseCyclusVoorAansturing { get; set; }
        public BitmapCoordinatenDataModel LichtenBitmapData { get; set; }
        public BitmapCoordinatenDataModel BellenBitmapData { get; set; }

        public bool ShouldSerializeLichtenBitmapData()
        {
            return Lichten;
        }

        public bool ShouldSerializeBellenBitmapData()
        {
            return Bellen;
        }

        public WaarschuwingsGroepModel()
        {
            LichtenBitmapData = new BitmapCoordinatenDataModel();
            BellenBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
