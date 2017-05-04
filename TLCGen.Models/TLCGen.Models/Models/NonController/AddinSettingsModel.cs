using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class AddinSettingsModel
    {
        public string Naam { get; set; }
        public List<AddinSettingsPropertyModel> Properties { get; set; }

        public AddinSettingsModel()
        {
            Properties = new List<AddinSettingsPropertyModel>();
        }
    }
}
