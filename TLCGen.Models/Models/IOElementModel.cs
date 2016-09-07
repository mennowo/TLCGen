using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public abstract class IOElementModel
    {
        [Browsable(false)]
        [XmlArrayItem(ElementName = "Coordinaat")]
        public List<BitmapCoordinaatModel> BitmapCoordinaten { get; set; }

        public IOElementModel()
        {
            BitmapCoordinaten = new List<BitmapCoordinaatModel>();
        }
    }
}
