using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public interface ITemplatesProvider
    {
        TLCGenTemplatesModel Templates { get; }

        void LoadSettings();
        void SaveSettings();
    }
}
