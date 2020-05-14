using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class VLOGCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmmaxtvgvlog;
        private CCOLGeneratorCodeStringSettingModel _prmmaxtfbvlog;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.Data.VLOGType != VLOGTypeEnum.Geen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmmaxtvgvlog.Setting, 5, CCOLElementTimeTypeEnum.CT_type, _prmmaxtvgvlog));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmmaxtfbvlog.Setting, 90, CCOLElementTimeTypeEnum.TS_type, _prmmaxtfbvlog));
            }
        }

        public override bool HasCCOLElements() => true;

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.TabCControlDefaults:
                    return 10;
                case CCOLCodeTypeEnum.TabCControlParameters:
                    return 0;
                case CCOLCodeTypeEnum.TabCBeforeIncludes:
                case CCOLCodeTypeEnum.RegCBeforeIncludes:
                    return 10;
                case CCOLCodeTypeEnum.RegCSystemApplication:
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    return 30;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.TabCControlDefaults:

                    if (c.Data.CCOLVersie <= Models.Enumerations.CCOLVersieEnum.CCOL8 &&
                        c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen)
                    {
                        sb.AppendLine($"#ifndef NO_VLOG");
                        sb.AppendLine($"{ts}MON_def = 1;");
                        sb.AppendLine($"{ts}LOG_def = 1;");
                        sb.AppendLine($"#endif");
                    }
                    else if (c.Data.CCOLVersie > Models.Enumerations.CCOLVersieEnum.CCOL8 &&
                             c.Data.VLOGSettings?.VLOGToepassen == true)
                    {
                        sb.AppendLine($"#ifndef NO_VLOG");
                        sb.AppendLine($"{ts}#if !defined NO_VLOG_300");
                        sb.AppendLine($"{ts}{ts}/* Defaults voor VLOG");
                        sb.AppendLine($"{ts}{ts}   Toelichting instellingen:");
                        sb.AppendLine($"{ts}{ts}   - MONDP   1=DP-status");
                        sb.AppendLine($"{ts}{ts}   - MONIS   1=IS-status, 2=IS-snelheid (ISV_type) 4=IS-lengte (ISL_type), 8=IS-mulv");
                        sb.AppendLine($"{ts}{ts}   - MONFC   1=FCWUS, 2=FCGUS, 4=FCMON2, 8=FCMON3, 32=OVMON5, 64=FCTIMING, 128=FCRWT");
                        sb.AppendLine($"{ts}{ts}   - MONUS   1=WUS-status, 2=GUS-status, 4=WUS-mulv, 8=GUS-mulv");
                        sb.AppendLine($"{ts}{ts}   - MONDS   1=DS-status");
                        sb.AppendLine($"{ts}{ts}*/");
                        sb.AppendLine($"{ts}{ts}MONTYPE_def = {c.Data.VLOGSettings.MONTYPE_def};");
                        sb.AppendLine($"{ts}{ts}MONDP_def   = {c.Data.VLOGSettings.MONDP_def};");
                        sb.AppendLine($"{ts}{ts}MONIS_def   = {c.Data.VLOGSettings.MONIS_def};");
                        sb.AppendLine($"{ts}{ts}MONFC_def   = {c.Data.VLOGSettings.MONFC_def};");
                        sb.AppendLine($"{ts}{ts}MONUS_def   = {c.Data.VLOGSettings.MONUS_def};");
                        sb.AppendLine($"{ts}{ts}MONDS_def   = {c.Data.VLOGSettings.MONDS_def};");
                        sb.AppendLine($"{ts}{ts}LOGTYPE_def = {c.Data.VLOGSettings.LOGTYPE_def};");
                        sb.AppendLine($"{ts}#else /* VLOG 2.0 */");
                        sb.AppendLine($"{ts}{ts}MON_def = 1;");
                        sb.AppendLine($"{ts}{ts}LOG_def = 1;");
                        sb.AppendLine($"{ts}#endif");
                        sb.AppendLine($"#endif");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.TabCControlParameters:
                    // dit zit in "CCOLGeneratorGenerateTabC.cs" omdat we hier een 
                    // lijst met CCOL uitgangen nodig hebben
                    return null;

                case CCOLCodeTypeEnum.TabCBeforeIncludes:
                case CCOLCodeTypeEnum.RegCBeforeIncludes:
                    if ((c.Data.CCOLVersie <= Models.Enumerations.CCOLVersieEnum.CCOL8 &&
                         c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen ||
                         c.Data.CCOLVersie > Models.Enumerations.CCOLVersieEnum.CCOL8 &&
                         c.Data.VLOGSettings?.VLOGToepassen == true) &&
                        !c.Data.VLOGInTestOmgeving)
                    {
                        sb.AppendLine("#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                        sb.AppendLine($"{ts}#define NO_VLOG");
                        sb.AppendLine("#endif");
                    }
                    return sb.ToString();
                    
                case CCOLCodeTypeEnum.RegCSystemApplication:
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    if (type == CCOLCodeTypeEnum.RegCSystemApplication && c.Data.CCOLVersie > CCOLVersieEnum.CCOL8) return "";
                    if (type == CCOLCodeTypeEnum.RegCSystemApplication2 && c.Data.CCOLVersie <= CCOLVersieEnum.CCOL8) return "";

                    if (c.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 && c.Data.VLOGType != Models.Enumerations.VLOGTypeEnum.Geen ||
                        c.Data.CCOLVersie > CCOLVersieEnum.CCOL8 && c.Data.VLOGSettings.VLOGToepassen)
                    {
                        sb.AppendLine($"#ifndef NO_VLOG");
                        sb.AppendLine($"{ts}mon3_mon4_buffers(SAPPLPROG, PRM[{_prmpf}{_prmmaxtvgvlog}], PRM[{_prmpf}{_prmmaxtfbvlog}]);");
                        if(c.PrioData.PrioIngrepen.Any() || c.PrioData.HDIngrepen.Any())
                        {
                            sb.AppendLine($"{ts}#ifndef NO_VLOG_200");
                            sb.AppendLine($"{ts}{ts}VLOG_mon5_buffer();");
                            sb.AppendLine($"{ts}#endif ");
                        }
                        if (c.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 && c.Data.VLOGType == Models.Enumerations.VLOGTypeEnum.Filebased ||
                            c.Data.CCOLVersie > CCOLVersieEnum.CCOL8 && 
                            (c.Data.VLOGSettings.LOGPRM_VLOGMODE == VLOGLogModeEnum.VLOGMODE_LOG_FILE_ASCII ||
                             c.Data.VLOGSettings.LOGPRM_VLOGMODE == VLOGLogModeEnum.VLOGMODE_LOG_FILE_BINAIR))
                        {
                            sb.AppendLine("#if (!defined AUTOMAAT) || (defined VISSIM)");
                            sb.AppendLine($"{ts}{ts}file_uber_to_file_hour(LOGFILE_NUMBER_MAX, LOGFILE_LENGTH_MAX);");
                            sb.AppendLine($"{ts}#endif");
                        }
                        sb.AppendLine($"#endif");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }
    }
}
