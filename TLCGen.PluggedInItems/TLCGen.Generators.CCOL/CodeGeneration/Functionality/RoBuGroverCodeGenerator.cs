using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [CCOLCodePieceGenerator]
    public class RoBuGroverCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmrgv;
        private CCOLGeneratorCodeStringSettingModel _prmmin_tcyclus;
        private CCOLGeneratorCodeStringSettingModel _prmmax_tcyclus;
        private CCOLGeneratorCodeStringSettingModel _prmmintvg;
        private CCOLGeneratorCodeStringSettingModel _prmmaxtvg;
        private CCOLGeneratorCodeStringSettingModel _prmtvg_omhoog;
        private CCOLGeneratorCodeStringSettingModel _prmtvg_omlaag;
        private CCOLGeneratorCodeStringSettingModel _prmtvg_verschil;
        private CCOLGeneratorCodeStringSettingModel _prmtvg_npr_omlaag;
        private CCOLGeneratorCodeStringSettingModel _hprreal;
        private CCOLGeneratorCodeStringSettingModel _hrgvact;
        private CCOLGeneratorCodeStringSettingModel _schrgv;
        private CCOLGeneratorCodeStringSettingModel _schrgv_snel;
        private CCOLGeneratorCodeStringSettingModel _usrgv;
        private CCOLGeneratorCodeStringSettingModel _tfd;
        private CCOLGeneratorCodeStringSettingModel _thd;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.RoBuGrover.ConflictGroepen?.Count == 0)
                return;

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmrgv.Setting, (int)c.RoBuGrover.MethodeRoBuGrover, CCOLElementTimeTypeEnum.None, _prmrgv));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmmin_tcyclus.Setting, c.RoBuGrover.MinimaleCyclustijd, CCOLElementTimeTypeEnum.TE_type, _prmmin_tcyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmmax_tcyclus.Setting, c.RoBuGrover.MaximaleCyclustijd, CCOLElementTimeTypeEnum.TE_type, _prmmax_tcyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmtvg_omhoog.Setting, c.RoBuGrover.GroenOphoogFactor, CCOLElementTimeTypeEnum.TE_type, _prmtvg_omhoog));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmtvg_omlaag.Setting, c.RoBuGrover.GroenVerlaagFactor, CCOLElementTimeTypeEnum.TE_type, _prmtvg_omlaag));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmtvg_verschil.Setting, c.RoBuGrover.GroentijdVerschil, CCOLElementTimeTypeEnum.TE_type, _prmtvg_verschil));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_prmtvg_npr_omlaag.Setting, c.RoBuGrover.GroenVerlaagFactorNietPrimair, CCOLElementTimeTypeEnum.TE_type, _prmtvg_npr_omlaag));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_hrgvact.Setting, _hrgvact));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_schrgv.Setting, c.RoBuGrover.RoBuGrover ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schrgv));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_schrgv_snel.Setting, c.RoBuGrover.OphogenTijdensGroen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schrgv_snel));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(_usrgv.Setting, _usrgv, c.RoBuGrover.BitmapData));

            foreach(var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                    continue;

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmintvg}_{fc.FaseCyclus}", fc.MinGroenTijd, CCOLElementTimeTypeEnum.TE_type, _prmmintvg, fc.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmaxtvg}_{fc.FaseCyclus}", fc.MaxGroenTijd, CCOLElementTimeTypeEnum.TE_type, _prmmaxtvg, fc.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprreal}{fc.FaseCyclus}", _hprreal, fc.FaseCyclus));
                foreach(var d in fc.FileDetectoren)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tfd}{_dpf}{d.Detector}", d.FileTijd, CCOLElementTimeTypeEnum.TE_type, _tfd, fc.FaseCyclus, d.Detector));
                }
                foreach (var d in fc.HiaatDetectoren)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thd}{_dpf}{d.Detector}", d.HiaatTijd, CCOLElementTimeTypeEnum.TE_type, _thd, fc.FaseCyclus, d.Detector));
                }
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCPreApplication => new []{60},
                CCOLCodeTypeEnum.RegCTop => new []{30},
                CCOLCodeTypeEnum.RegCVerlenggroen => new []{35},
                CCOLCodeTypeEnum.RegCMaxgroen => new []{30},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            if(c.RoBuGrover.ConflictGroepen?.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            switch(type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine($"{ts}/* Robuuste Groenverdeler */");
                    sb.AppendLine($"{ts}#include \"{c.Data.Naam}rgv.c\"");
                    sb.AppendLine($"{ts}{c.GetBoolV()} rgvinit = TRUE;");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}/* Robuuste Groenverdeler */");
                    sb.AppendLine($"{ts}IH[{_hpf}{_hrgvact}] = SCH[{_schpf}{_schrgv}];");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    sb.AppendLine($"{ts}/* AANROEP EN RAPPOTEREN ROBUGROVER */");
                    sb.AppendLine($"{ts}if (IH[{_hpf}{_hrgvact}] != 0)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}int teller = 0;");
                    if (c.Data.SynchronisatiesType == Models.Enumerations.SynchronisatiesTypeEnum.InterFunc)
                    {
                        sb.AppendLine($"{ts}{ts}int i;");
                        sb.AppendLine($"{ts}{ts}if (rgvinit)");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}for (i = 0; i < FCMAX; ++i) TVG_rgv[i] = TVG_max[i];");
                        sb.AppendLine($"{ts}{ts}{ts}rgvinit = 0;");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}/* kopieer de basis waarden van TVG_max */");
                        sb.AppendLine($"{ts}{ts}/* ------------------------------------ */");
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < FCMAX; ++i) TVG_basis[i] = TVG_max[i] > 0 ? TVG_max[i] : 1;");
                        sb.AppendLine($"{ts}{ts}BepaalInterStartGroenTijden_rgv();");
                    }
                    sb.AppendLine();
                    foreach(var cg in c.RoBuGrover.ConflictGroepen)
                    {
                        sb.Append($"{ts}{ts}TC[teller++] = berekencyclustijd_ISG_va_arg(");
                        foreach(var fc in cg.Fasen)
                        {
                            sb.Append($"{_fcpf}{fc.FaseCyclus}, ");
                        }
                        sb.AppendLine($"END);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}TC_max = TC[0];");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}for (teller = 1; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}if (TC_max < TC[teller])");
                    sb.AppendLine($"{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}TC_max = TC[teller];");
                    sb.AppendLine($"{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}/* RoBuGrover verklikking in F11 scherm");
                    sb.AppendLine($"{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                    sb.AppendLine($"{ts}{ts}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(52, teller + 6, \"%4d\", TC[teller]);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}#endif");
                    sb.AppendLine($"{ts}*/");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* AANROEP ROBUUSTE GROENTIJD VERDELER */");
                    sb.AppendLine($"{ts}{ts}/* ================================== */");
                    sb.AppendLine($"{ts}{ts}rgv_add();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usrgv}] = TRUE;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}else");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in c.Fasen)
                    {
                        if(fc.Type == Models.Enumerations.FaseTypeEnum.Auto)
                        {
                            sb.AppendLine($"{ts}{ts}TVG_rgv[{_fcpf}{fc.Naam}] = TVG_basis[{_fcpf}{fc.Naam}];");
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usrgv}] = FALSE;");
                    sb.AppendLine($"{ts}}}");

                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
