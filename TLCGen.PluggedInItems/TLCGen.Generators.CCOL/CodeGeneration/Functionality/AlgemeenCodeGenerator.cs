using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class AlgemeenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private string _hplact;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schtoon7s;
        private CCOLGeneratorCodeStringSettingModel _ussegm;
        private CCOLGeneratorCodeStringSettingModel _usML;
        private CCOLGeneratorCodeStringSettingModel _prmxx;
        private CCOLGeneratorCodeStringSettingModel _prmyy;
        private CCOLGeneratorCodeStringSettingModel _prmzz;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            // Segment display elements
            foreach (var item in c.Data.SegmentenDisplayBitmapData)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(item.BitmapData, $"{_uspf}{_ussegm}{item.Naam}"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ussegm}{item.Naam}", _ussegm));
            }
            if (c.Data.SegmentDisplayType == Models.Enumerations.SegmentDisplayTypeEnum.DrieCijferDisplay)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtoon7s}", 1, CCOLElementTimeTypeEnum.SCH_type, _schtoon7s));
            }

            // Module display elements
            if (c.Data.UitgangPerModule)
            {
                foreach (var item in c.Data.ModulenDisplayBitmapData)
                {
                    _myBitmapOutputs.Add(new CCOLIOElement(item.BitmapData, $"{_uspf}{item.Naam.Replace("ML", _usML.Setting)}"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{item.Naam.Replace("ML", _usML.Setting)}", _usML, item.Naam.Replace("ML", _usML.Setting)));
                }
            }

            // Inputs
            foreach (var i in c.Ingangen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(i.Naam, CCOLElementTypeEnum.Ingang, i.Omschrijving));
                _myBitmapInputs.Add(new CCOLIOElement(i, $"{_ispf}{i.Naam}"));
            }

            // Versie beheer
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmxx}", c.Data.HuidigeVersieMajor, CCOLElementTimeTypeEnum.None, _prmxx));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmyy}", c.Data.HuidigeVersieMinor, CCOLElementTimeTypeEnum.None, _prmyy));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmzz}", c.Data.HuidigeVersieRevision, CCOLElementTimeTypeEnum.None, _prmzz));
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapOutputs() => true;

        public override bool HasCCOLBitmapInputs() => true;

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    return 10;
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return 40;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 30;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 20;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    if (!c.Data.PracticeOmgeving) return "";
                    sb.AppendLine($"/* T.b.v. practice */");
                    sb.AppendLine($"#ifdef PRACTICE_TEST");
                    sb.AppendLine($"{ts}#define XTND_DIC");
                    sb.AppendLine($"#endif");
                    sb.AppendLine($"");
                    sb.AppendLine($"");
                    sb.AppendLine($"");
                    sb.AppendLine($"");

                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    if (!c.Data.GenererenDuurtestCode) return "";
                    sb.AppendLine($"{ts}/* TESTOMGEVING */");
                    sb.AppendLine($"{ts}/* ============ */");
                    sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST && !defined VISSIM)");
                    sb.AppendLine($"{ts}{ts}if (!SAPPLPROG)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}#ifdef DUURTEST");
                    sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F5KEY); ");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(ALTF9KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(F2KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(CTRLF3KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(F4KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F10KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F11KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}CFB_max = 0; /* maximum aantal herstarts na fasebewaking */");
                    sb.AppendLine($"{ts}{ts}{ts}MONTYPE[MONTYPE_DATI] = 0;");
                    sb.AppendLine($"{ts}{ts}{ts}LOGTYPE[LOGTYPE_DATI] = 0;");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (!c.Data.GenererenDuurtestCode) return "";
                    sb.AppendLine($"{ts}{ts}#ifdef DUURTEST");
                    sb.AppendLine($"{ts}{ts}for (int i = 0; i < FCMAX; ++i)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}if (TFB_timer[i] + 3 > PRM[prmfb])");
                    sb.AppendLine($"{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}stuffkey(F5KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (!c.Data.GenererenDuurtestCode) return "";
                    sb.AppendLine($"{ts}/* TESTOMGEVING */");
                    sb.AppendLine($"{ts}/* ============ */");
                    sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST && !defined VISSIM)");
                    sb.AppendLine($"{ts}{ts}if (TS &&");
                    sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_JAAR] == 2099) &&");
                    sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_MAAND] == 1) &&");
                    sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_DAG] == 1) &&");
                    sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_UUR] == 1) &&");
                    sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_MINUUT] == 1) &&");
                    sb.AppendLine($"{ts}{ts}    (CIF_KLOK[CIF_SECONDE] == 1))");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(F3KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(F5KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}stuffkey(F4KEY);");
                    sb.AppendLine($"{ts}{ts}{ts}//stuffkey(F10KEY); ");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.EnkelDisplay)
                    {
                        if (!c.Data.MultiModuleReeksen)
                        {
                            sb.AppendLine($"{ts}SegmentSturing(ML+1, {_uspf}{_ussegm}1, {_uspf}{_ussegm}2, {_uspf}{_ussegm}3, {_uspf}{_ussegm}4, {_uspf}{_ussegm}5, {_uspf}{_ussegm}6, {_uspf}{_ussegm}7);");
                        }
                    }
                    else if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.DrieCijferDisplay)
                    {
                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}] && SCH[{_schpf}{_schtoon7s}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}/* Uitsturen segmenten verklikking signaalplantijd */");
                            sb.AppendLine($"{ts}{ts}SegmentSturingDrie(TX_timer,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
                            sb.AppendLine($"{ts}}}");
                            if (!c.Data.MultiModuleReeksen)
                            {
                                sb.AppendLine($"{ts}else if (SCH[{_schpf}{_schtoon7s}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}/* Uitsturen segmenten verklikking module regelen */");
                                sb.AppendLine($"{ts}{ts}SegmentSturingDrie(ML + 1,");
                                sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
                                sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
                                sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
                                sb.AppendLine($"{ts}}}");
                            }
                        }
                        else
                        {
                            if (!c.Data.MultiModuleReeksen)
                            {
                                sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtoon7s}])");
                                sb.AppendLine($"{ts}{{");
                                sb.AppendLine($"{ts}{ts}SegmentSturingDrie(ML + 1,");
                                sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
                                sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
                                sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
                                sb.AppendLine($"{ts}}}");
                            }
                        }
                    }
                    sb.AppendLine();
                    if (c.Data.UitgangPerModule && c.Data.ModulenDisplayBitmapData.Any())
                    {
                        foreach(var m in c.Data.ModulenDisplayBitmapData)
                        {
                            var mNaam = Regex.Replace(m.Naam, @"ML[A-E]+", "ML");
                            var mReeks = Regex.Replace(m.Naam, @"ML([A-E]?)[0-9]+", "ML$1");
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{m.Naam.Replace("ML", _usML.Setting)}] = {mReeks} == {mNaam};");
                        }
                    }

                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            return base.SetSettings(settings);
        }
    }
}
