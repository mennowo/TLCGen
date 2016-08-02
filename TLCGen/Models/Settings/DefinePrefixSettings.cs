using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class DefinePrefixSettings
    {
        public StringSettingModel FaseCyclusDefinePrefix { get; set; }
        public StringSettingModel DetectorDefinePrefix { get; set; }
        public StringSettingModel UitgangDefinePrefix { get; set; }
        public StringSettingModel IngangDefinePrefix { get; set; }
        public StringSettingModel HulpElementDefinePrefix { get; set; }
        public StringSettingModel TimerDefinePrefix { get; set; }
        public StringSettingModel CounterDefinePrefix { get; set; }
        public StringSettingModel SchakelaarDefinePrefix { get; set; }
        public StringSettingModel MemoryElementDefinePrefix { get; set; }
        public StringSettingModel ParameterDefinePrefix { get; set; }

        public DefinePrefixSettings()
        {
            FaseCyclusDefinePrefix = new StringSettingModel("fc", "Fase #define prefix");
            DetectorDefinePrefix = new StringSettingModel("d", "Detector #define prefix");
            UitgangDefinePrefix = new StringSettingModel("us", "Uitgang #define prefix");
            IngangDefinePrefix = new StringSettingModel("is", "Ingang #define prefix");
            HulpElementDefinePrefix = new StringSettingModel("h", "Hulp element #define prefix");
            TimerDefinePrefix = new StringSettingModel("t", "Timer #define prefix");
            CounterDefinePrefix = new StringSettingModel("c", "Counter #define prefix");
            SchakelaarDefinePrefix = new StringSettingModel("sch", "Schakelaar #define prefix");
            MemoryElementDefinePrefix = new StringSettingModel("m", "Memory element #define prefix");
            ParameterDefinePrefix = new StringSettingModel("prm", "Parameter #define prefix");
        }
    }
}
