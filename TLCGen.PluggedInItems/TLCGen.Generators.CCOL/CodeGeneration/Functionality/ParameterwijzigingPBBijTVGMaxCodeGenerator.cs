using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class ParameterwijzigingPBBijTVGMaxCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private string _mperiod;
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _mwijzpb;
        private CCOLGeneratorCodeStringSettingModel _maantalvgtwijzpb;
#pragma warning restore 0649
        
        #endregion // Fields
        
        #region CCOLCodePieceGeneratorBase Overrides

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            if (!c.Data.ParameterwijzigingPBBijTVGMax) return;
            
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mwijzpb}", _mwijzpb));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_maantalvgtwijzpb}", _maantalvgtwijzpb));
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.SysHBeforeUserDefines => new []{20},
                CCOLCodeTypeEnum.RegCTop => new []{60},
                CCOLCodeTypeEnum.RegCVerlenggroenNaAdd => new []{30},
                CCOLCodeTypeEnum.RegCMaxgroenNaAdd => new []{30},
                CCOLCodeTypeEnum.TabCTop => new []{10},
                CCOLCodeTypeEnum.TabCControlParameters => new []{20},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            if (!c.Data.ParameterwijzigingPBBijTVGMax) return null;
            
            var sb = new StringBuilder();
            var tg = c.Data.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "mg" : "vg";

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    var i = 0;
                    foreach (var sg in c.Fasen)
                    {
                        if (c.GroentijdenSets.Any(x => x.Groentijden.Any(x2 => x2.Waarde != null && x2.FaseCyclus == sg.Naam)))
                        {
                            sb.AppendLine($"{ts}#define tvgmaxprm{sg.Naam} {i} /* {_fcpf}{sg.Naam} heeft {_prmpf}{tg}#_{sg.Naam} parameters */");
                            ++i;
                        }
                    }

                    sb.AppendLine($"{ts}#define aanttvgmaxprm {i} /* aantal {_fcpf} met max. verlenggroenparameters ({_prmpf}{tg}#_$$, ..)  */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine("mulv itvgmaxprm[aanttvgmaxprm]; /* fasecycli met max. verlenggroen parameter */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCMaxgroenNaAdd:
                case CCOLCodeTypeEnum.RegCVerlenggroenNaAdd:
                    var firstSet = c.GroentijdenSets.FirstOrDefault(x => x.Groentijden.Any(x2 => x2.Waarde != null));
                    if (firstSet == null) return null;
                    if (c.Data.TVGAMaxAlsDefaultGroentijdSet && firstSet.Naam == c.PeriodenData.DefaultPeriodeGroentijdenSet)
                    {
                        firstSet = c.GroentijdenSets.FirstOrDefault(x => x.Naam != c.PeriodenData.DefaultPeriodeGroentijdenSet && x.Groentijden.Any(x2 => x2.Waarde != null));
                        if (firstSet == null) return null;
                    }
                    var firstSetGt = firstSet.Groentijden.FirstOrDefault(x => x.Waarde != null);
                    if (firstSetGt == null) return null;
                    sb.AppendLine($"{ts}if(CIF_PARM1WIJZPB != CIF_GEEN_PARMWIJZ)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}MM[{_mpf}{_mwijzpb}]= CIF_PARM1WIJZPB; /* indexnummer van gewijzigde max.verlengroentijd of andere gewijzigde parameter uit PARM1 buffer */");
                    sb.AppendLine($"{ts}{ts}if (set_parm1wijzpb_tvgmax (MM[{_mpf}{_mperiod}], {_prmpf}{firstSet.Naam.ToLower()}_{firstSetGt.FaseCyclus}, itvgmaxprm, aanttvgmaxprm)) /* argumenten: actuele periode, index eerste verlenggroen parameter, array van fc met prmvg#_$$ */");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}++MM[{_mpf}{_maantalvgtwijzpb}];");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();
                case CCOLCodeTypeEnum.TabCTop:
                    sb.AppendLine("extern int itvgmaxprm[]; /* fasecycli met max. verlenggroen parameter (gedeclareerd in reg.c) */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.TabCControlParameters:
                    foreach (var sg in c.Fasen)
                    {
                        if (c.GroentijdenSets.Any(x => x.Groentijden.Any(x2 => x2.Waarde != null && x2.FaseCyclus == sg.Naam)))
                        {
                            sb.AppendLine($"{ts}itvgmaxprm[tvgmaxprm{sg.Naam}] = {_fcpf}{sg.Naam};");
                        }
                    }
                    return sb.ToString();
            }

            return null;
        }
        
        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _mperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("mperiod");

            return base.SetSettings(settings);
        }
        
        #endregion // CCOLCodePieceGeneratorBase Overrides

    }
}