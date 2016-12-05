using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Models.Settings;

namespace TLCGen.Settings
{
    public interface ISettingsProvider
    {
        TLCGenSettingsModel Settings { get; set; }

        void LoadApplicationSettings();
        void SaveApplicationSettings();

        string GetFaseCyclusDefinePrefix();
        string GetDetectorDefinePrefix();

        void ApplyDefaultFaseCyclusSettings(FaseCyclusModel fcm, string define);
        void ApplyDefaultFaseCyclusSettings(FaseCyclusModel fcm, FaseTypeEnum type);
    }
}
