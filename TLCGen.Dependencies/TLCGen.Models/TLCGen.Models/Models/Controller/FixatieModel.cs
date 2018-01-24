using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FixatieModel
    {
        public bool FixatieMogelijk { get; set; }
        public bool BijkomenTijdensFixatie { get; set; }

        [Browsable(false)]
        [IOElement("fix", BitmappedItemTypeEnum.Ingang, "", "FixatieMogelijk")]
        public BitmapCoordinatenDataModel FixatieBitmapData { get; set; }

        public FixatieModel()
        {
            FixatieBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
