using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Settings
{
    [Serializable]
    public abstract class TemplateModelBase
    {
        public string Naam { get; set; }
        public string Replace { get; set; }

        public abstract List<object> GetItems();
    }
}
