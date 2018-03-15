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
    public class VLOGCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmmaxtvgvlog;
        private CCOLGeneratorCodeStringSettingModel _prmmaxtfbvlog;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            if (c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
            {
                _MyElements.Add(new CCOLElement(_prmmaxtvgvlog.Setting, 5, CCOLElementTimeTypeEnum.CT_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement(_prmmaxtfbvlog.Setting, 90, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Parameter));
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

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    return 20;
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 30;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    if (!c.Data.VLOGInTestOmgeving)
                    {
                        sb.AppendLine("#ifndef AUTOMAAT");
                        sb.AppendLine($"{ts}#define NO_VLOG");
                        sb.AppendLine("#endif");
                        sb.AppendLine();
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    if (c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
                    {
                        sb.AppendLine($"#ifndef NO_VLOG");
                        sb.AppendLine($"{ts}mon3_mon4_buffers(SAPPLPROG, PRM[{_prmpf}{_prmmaxtvgvlog}], PRM[{_prmpf}{_prmmaxtfbvlog}]);");
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

        //public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        //{
        //    if (settings?.Settings == null)
        //    {
        //        return false;
        //    }
        //
        //    foreach (var s in settings.Settings)
        //    {
        //        switch (s.Default)
        //        {
        //            case "maxtvg": _prmmaxtvgvlog = s.Setting ?? s.Default; break;
        //            case "maxtfb": _prmmaxtfb = s.Setting ?? s.Default; break;
        //        }
        //    }
        //
        //    return base.SetSettings(settings);
        //}
    }
}
