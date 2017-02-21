using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public class FixatieModel
    {
        public bool FixatieMogelijk { get; set; }
        public bool BijkomenTijdensFixatie { get; set; }

        [IOElement("fix", BitmappedItemTypeEnum.Ingang, "", "FixatieMogelijk")]
        public BitmapCoordinatenDataModel FixatieBitmapData { get; set; }

        public FixatieModel()
        {
            FixatieBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
