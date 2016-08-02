using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class ApplicationSettings
    {
        public DefinePrefixSettings PrefixSettings;
        public List<SettingModelBase> PrefixSettingsList { get; set; }

        public ApplicationSettings()
        {
            PrefixSettings = new DefinePrefixSettings();

            PrefixSettingsList = new List<SettingModelBase>();
            PrefixSettingsList.Add(PrefixSettings.FaseCyclusDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.DetectorDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.UitgangDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.IngangDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.HulpElementDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.TimerDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.CounterDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.SchakelaarDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.MemoryElementDefinePrefix);
            PrefixSettingsList.Add(PrefixSettings.ParameterDefinePrefix);
        }
    }
}
