using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Settings;

namespace TLCGen.Helpers.Settings
{
    public static class SettingsProvider
    {
        public static ApplicationSettings AppSettings
        {
            get;
            set;
        }

        static SettingsProvider()
        {
            AppSettings = new ApplicationSettings();
        }
    }
}
