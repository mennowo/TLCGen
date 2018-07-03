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
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

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
        private CCOLGeneratorCodeStringSettingModel _schrgv;
        private CCOLGeneratorCodeStringSettingModel _schrgv_snel;
        private CCOLGeneratorCodeStringSettingModel _usrgv;
        private CCOLGeneratorCodeStringSettingModel _tfd;
        private CCOLGeneratorCodeStringSettingModel _thd;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            if (c.RoBuGrover.ConflictGroepen?.Count == 0)
                return;

            _MyElements.Add(new CCOLElement(_prmrgv.Setting, (int)c.RoBuGrover.MethodeRoBuGrover, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_prmmin_tcyclus.Setting, c.RoBuGrover.MinimaleCyclustijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_prmmax_tcyclus.Setting, c.RoBuGrover.MaximaleCyclustijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_prmtvg_omhoog.Setting, c.RoBuGrover.GroenOphoogFactor, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_prmtvg_omlaag.Setting, c.RoBuGrover.GroenVerlaagFactor, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_prmtvg_verschil.Setting, c.RoBuGrover.GroentijdVerschil, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_prmtvg_npr_omlaag.Setting, c.RoBuGrover.GroenVerlaagFactorNietPrimair, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
            _MyElements.Add(new CCOLElement(_schrgv.Setting, c.RoBuGrover.RoBuGrover ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
            _MyElements.Add(new CCOLElement(_schrgv_snel.Setting, c.RoBuGrover.OphogenTijdensGroen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
            _MyElements.Add(new CCOLElement(_usrgv.Setting, CCOLElementTypeEnum.Uitgang));

            foreach(var fc in c.RoBuGrover.SignaalGroepInstellingen)
            {
                if (fc.FileDetectoren.Count == 0 && fc.HiaatDetectoren.Count == 0)
                    continue;

                _MyElements.Add(new CCOLElement($"{_prmmintvg}_{fc.FaseCyclus}", fc.MinGroenTijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmmaxtvg}_{fc.FaseCyclus}", fc.MaxGroenTijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_hprreal}{fc.FaseCyclus}", CCOLElementTypeEnum.HulpElement));
                foreach(var d in fc.FileDetectoren)
                {
                    _MyElements.Add(new CCOLElement($"{_tfd}{_dpf}{d.Detector}", d.FileTijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }
                foreach (var d in fc.HiaatDetectoren)
                {
                    _MyElements.Add(new CCOLElement($"{_thd}{_dpf}{d.Detector}", d.HiaatTijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }
            }

            _MyBitmapOutputs.Add(new CCOLIOElement(c.RoBuGrover.BitmapData as IOElementModel, _uspf + _usrgv));
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    return 30;
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    return 30;
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return 30;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if(c.RoBuGrover.ConflictGroepen?.Count == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            switch(type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine($"{ts}/* Robuuste Groenverdeler */");
                    sb.AppendLine($"{ts}#include \"{c.Data.Naam}rgv.c\"");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    sb.AppendLine($"{ts}/* AANROEP EN RAPPOTEREN ROBUGROVER */");
                    sb.AppendLine($"{ts}if (SCH[{_schpf}{_schrgv}] != 0)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}int teller = 0;");
                    sb.AppendLine();
                    foreach(var cg in c.RoBuGrover.ConflictGroepen)
                    {
                        sb.Append($"{ts}{ts}TC[teller++] = berekencyclustijd_va_arg(");
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
                    sb.AppendLine($"{ts}#if !defined AUTOMAAT");
                    sb.AppendLine($"{ts}{ts}for (teller = 0; teller < MAX_AANTAL_CONFLICTGROEPEN; ++teller)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}xyprintf(50, teller + 1, \"%4d\", TC[teller]);");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}#endif");
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
                    sb.AppendLine();

                    return sb.ToString();

                default:
                    return null;
            }
        }
    }
}
