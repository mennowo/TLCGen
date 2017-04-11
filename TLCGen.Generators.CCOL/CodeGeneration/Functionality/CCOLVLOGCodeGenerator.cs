using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLVLOGCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        private string _prmmaxtvgvlog;
        private string _prmmaxtfb;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            if (c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
            {
                _MyElements.Add(new CCOLElement(_prmmaxtvgvlog, 5, CCOLElementTimeTypeEnum.CT_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement(_prmmaxtfb, 90, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Parameter));
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                    if (!c.Data.VLOGInTestOmgeving)
                    {
                        sb.AppendLine("#define NO_VLOG");
                        sb.AppendLine();
                    }
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.SystemApplication:
                    if (c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
                    {
                        sb.AppendLine($"#ifndef NO_VLOG");
                        sb.AppendLine($"{ts}mon3_mon4_buffers(SAPPLPROG, PRM[{_prmpf}{_prmmaxtvgvlog}], PRM[{_prmpf}{_prmmaxtfb}]);");
                        if (c.Data.VLOGType == Models.Enumerations.VLOGTypeEnum.Filebased)
                        {
                            sb.AppendLine($"{ts}#ifndef AUTOMAAT");
                            sb.AppendLine($"{ts}{ts}file_uber_to_file_hour(LOGFILE_NUMBER_MAX, LOGFILE_LENGTH_MAX);");
                            sb.AppendLine($"{ts}#endif");
                        }
                        sb.AppendLine($"#endif");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            if (settings == null || settings.Settings == null)
            {
                return false;
            }

            foreach (var s in settings.Settings)
            {
                switch (s.Default)
                {
                    case "maxtvg": _prmmaxtvgvlog = s.Setting == null ? s.Default : s.Setting; break;
                    case "maxtfb": _prmmaxtfb = s.Setting == null ? s.Default : s.Setting; break;
                }
            }

            return base.SetSettings(settings);
        }
    }
}
