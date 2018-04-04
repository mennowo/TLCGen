using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Settings
{
    //public class TLCGenTemplateModelBaseWithLocation
    //{
    //    public string Location { get; set; }
    //    TLCGenTemplateModelBase Template { get; set; }
    //}

    public class TLCGenTemplatesModelWithLocation
    {
        public string Location { get; set; }
        public bool Editable { get; set; }
        public TLCGenTemplatesModel Templates { get; set; }
    }
    
}
