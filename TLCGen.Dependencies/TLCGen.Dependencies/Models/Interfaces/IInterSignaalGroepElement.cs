using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public interface IInterSignaalGroepElement
    {
        string FaseVan { get; set; }
        string FaseNaar { get; set; }
    }
}
