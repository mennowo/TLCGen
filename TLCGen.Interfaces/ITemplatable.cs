using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Interfaces
{
    public interface ITemplatable
    {
        string GetIdentifyingName();
        void ClearAllReferences();
        void SetAllIdentifyingNames(string search, string replace);
    }
}
