using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Settings
{
    [Serializable]
    public class DetectorTemplateModel : TemplateModelBase
    {
        public List<DetectorModel> Detectoren { get; set; }

        public DetectorTemplateModel()
        {
            Detectoren = new List<DetectorModel>();
        }

        public override List<object> GetItems()
        {
            List<object> items = new List<object>();
            foreach (var fc in Detectoren)
            {
                items.Add(fc);
            }
            return items;
        }
    }
}
