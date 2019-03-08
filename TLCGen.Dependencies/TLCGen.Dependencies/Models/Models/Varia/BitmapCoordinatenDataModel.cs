using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class BitmapCoordinatenDataModel : IOElementModel
    {
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
    }
}
