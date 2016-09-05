using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public class TLCGenSettingsModel
    {
        public CustomDataModel CustomData { get; set; }

        public TLCGenSettingsModel()
        {
            CustomData = new CustomDataModel();
        }
    }
}
