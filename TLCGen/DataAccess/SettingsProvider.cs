using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Settings;

namespace TLCGen.DataAccess
{
    public static class SettingsProvider
    {
        
        public static string GetFaseCyclusDefinePrefix()
        {
            return "fc";
        }

        public static string GetDetectorDefinePrefix()
        {
            return "d";
        }

        static SettingsProvider()
        {
        }
    }
}
