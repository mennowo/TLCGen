using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class GelijkstartenVoorstartenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tgsot;
        private CCOLGeneratorCodeStringSettingModel _schgs;
        private CCOLGeneratorCodeStringSettingModel _tvs;
        private CCOLGeneratorCodeStringSettingModel _tvsot;
#pragma warning restore 0649

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.SyncFunc) return;

            foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
            {
                if (gs.DeelConflict)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tgsot}{gs.FaseVan}{gs.FaseNaar}",
                            gs.GelijkstartOntruimingstijdFaseVan,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tgsot, gs.FaseVan, gs.FaseNaar));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tgsot}{gs.FaseNaar}{gs.FaseVan}",
                            gs.GelijkstartOntruimingstijdFaseNaar,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tgsot, gs.FaseNaar, gs.FaseVan));
                }
                if (gs.Schakelbaar != Models.Enumerations.AltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schgs}{gs.FaseVan}{gs.FaseNaar}",
                            gs.Schakelbaar == Models.Enumerations.AltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schgs, gs.FaseNaar, gs.FaseVan));
                }
            }

            foreach(var vs in c.InterSignaalGroep.Voorstarten)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tvs}{vs.FaseVan}{vs.FaseNaar}",
                        vs.VoorstartTijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tvs, vs.FaseVan, vs.FaseNaar));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tvsot}{vs.FaseNaar}{vs.FaseVan}",
                        vs.VoorstartOntruimingstijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tvsot, vs.FaseVan, vs.FaseNaar));
            }
        }

        public override bool HasCCOLElements() => true;
        
        public override bool HasFunctionLocalVariables() => true;
        
        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.SyncFunc ||
                        c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCSynchronisaties => 10,
                CCOLCodeTypeEnum.RegCAlternatieven => 30,
                CCOLCodeTypeEnum.PrioCIncludes => 10,
                _ => 0
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            // return if no sync
            if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.SyncFunc 
                || c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0)
                return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    // bits reset
                    sb.AppendLine($"{ts}/* reset synchronisatiebits. */");
                    sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
                    sb.AppendLine($"{ts}{{   RR[fc]&= ~(BIT1|BIT2|BIT3);");
                    sb.AppendLine($"{ts}{ts}RW[fc]&= ~(BIT3);");
                    sb.AppendLine($"{ts}{ts}YV[fc]&= ~(BIT1);");
                    sb.AppendLine($"{ts}{ts}YM[fc]&= ~(BIT3);");
                    sb.AppendLine($"{ts}{ts} X[fc]&= ~(BIT1|BIT2|BIT3);");
                    if(c.InterSignaalGroep.Gelijkstarten.Any() || c.InterSignaalGroep.Voorstarten.Any())
                    {
                        sb.AppendLine($"{ts}{ts}KR[fc]&= ~(BIT0|BIT1|BIT2|BIT3|BIT4|BIT5|BIT6|BIT7);");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}control_realisation_timers();");
                    sb.AppendLine();

                    // fictitious intergreen
                    if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.DeelConflict) ||
                        c.InterSignaalGroep.Voorstarten.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* (Her)start fictieve ontruimingstijden */");
                        sb.AppendLine($"{ts}/* ------------------------------------- */");
                        foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                        {
                            if (gs.DeelConflict)
                            {
                                sb.AppendLine($"{ts}FictiefOntruimen(({c.GetBoolV()}) TRUE, {_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, {_tpf}{_tgsot}{gs.FaseNaar}{gs.FaseVan}, BIT3);");
                                sb.AppendLine($"{ts}FictiefOntruimen(({c.GetBoolV()}) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, {_tpf}{_tgsot}{gs.FaseVan}{gs.FaseNaar}, BIT3);");
                            }
                        }
                        foreach (var vs in c.InterSignaalGroep.Voorstarten)
                        {
                            sb.AppendLine($"{ts}FictiefOntruimen(({c.GetBoolV()}) TRUE, {_fcpf}{vs.FaseNaar}, {_fcpf}{vs.FaseVan}, {_tpf}{_tvsot}{vs.FaseNaar}{vs.FaseVan}, BIT3);");
                        }
                        sb.AppendLine();

                        sb.AppendLine($"{ts}/* Corrigeer o.b.v. fictieve ontruimingstijden */");
                        sb.AppendLine($"{ts}/* ------------------------------------------- */");
                        foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                        {
                            if (gs.DeelConflict)
                            {
                                sb.AppendLine($"{ts}FictiefOntruimen_correctionKR(({c.GetBoolV()}) TRUE, {_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, {_tpf}{_tgsot}{gs.FaseNaar}{gs.FaseVan});");
                                sb.AppendLine($"{ts}FictiefOntruimen_correctionKR(({c.GetBoolV()}) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, {_tpf}{_tgsot}{gs.FaseVan}{gs.FaseNaar});");
                            }
                        }
                        foreach (var vs in c.InterSignaalGroep.Voorstarten)
                        {
                            sb.AppendLine($"{ts}FictiefOntruimen_correctionKR(({c.GetBoolV()}) TRUE, {_fcpf}{vs.FaseNaar}, {_fcpf}{vs.FaseVan}, {_tpf}{_tvsot}{vs.FaseNaar}{vs.FaseVan});");
                        }
                        sb.AppendLine();
                    }
                    // timers
                    sb.AppendLine($"{ts}/* Correcties op realisatietimers */");
                    sb.AppendLine($"{ts}/* ------------------------------ */");
                    foreach(var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        if(gs.Schakelbaar == Models.Enumerations.AltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}GelijkStarten_correctionKR(({c.GetBoolV()}) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar});");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}GelijkStarten_correctionKR(({c.GetBoolV()}) SCH[{_schpf}{_schgs}{gs.FaseVan}{gs.FaseNaar}], {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar});");
                        }
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}VoorStarten_correctionKR(({c.GetBoolV()}) TRUE, {_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, {_tpf}{_tvs}{vs.FaseVan}{vs.FaseNaar});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Gelijk Starten */");
                    sb.AppendLine($"{ts}/* -------------- */");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        if (gs.Schakelbaar == Models.Enumerations.AltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}GelijkStarten(({c.GetBoolV()}) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, BIT1, ({c.GetBoolV()}) FALSE);");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}GelijkStarten(({c.GetBoolV()}) SCH[{_schpf}{_schgs}{gs.FaseVan}{gs.FaseNaar}], {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, BIT1, ({c.GetBoolV()}) FALSE);");
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Voorstarten */");
                    sb.AppendLine($"{ts}/* ----------- */");
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}VoorStarten(({c.GetBoolV()}) TRUE, {_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, {_tpf}{_tvs}{vs.FaseVan}{vs.FaseNaar}, BIT3);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}realisation_timers(BIT4);");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCAlternatieven:
					sb.AppendLine($"{ts}/* set meerealisatie voor gelijk- of voorstartende richtingen */");
                    sb.AppendLine($"{ts}/* ---------------------------------------------------------- */");
                    foreach(var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}set_MRLW({_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, ({c.GetBoolV()}) (RA[{_fcpf}{vs.FaseNaar}] && (PR[{_fcpf}{vs.FaseNaar}] || AR[{_fcpf}{vs.FaseNaar}]) && A[{_fcpf}{vs.FaseVan}] && R[{_fcpf}{vs.FaseVan}] && !TRG[{_fcpf}{vs.FaseVan}] && !kcv({_fcpf}{vs.FaseVan})));");
                    }
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        if (gs.Schakelbaar == Models.Enumerations.AltijdAanUitEnum.Altijd)
                        {
                            sb.AppendLine($"{ts}set_MRLW({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseNaar}] || SG[{_fcpf}{gs.FaseNaar}]) && (PR[{_fcpf}{gs.FaseNaar}] || AR[{_fcpf}{gs.FaseNaar}]) && A[{_fcpf}{gs.FaseVan}] && R[{_fcpf}{gs.FaseVan}] && !TRG[{_fcpf}{gs.FaseVan}] && !kcv({_fcpf}{gs.FaseVan})));");
                            sb.AppendLine($"{ts}set_MRLW({_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseVan}] || SG[{_fcpf}{gs.FaseVan}]) && (PR[{_fcpf}{gs.FaseVan}] || AR[{_fcpf}{gs.FaseVan}]) && A[{_fcpf}{gs.FaseNaar}] && R[{_fcpf}{gs.FaseNaar}] && !TRG[{_fcpf}{gs.FaseNaar}] && !kcv({_fcpf}{gs.FaseNaar})));");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schgs}{gs.FaseVan}{gs.FaseNaar}]) set_MRLW({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseNaar}] || SG[{_fcpf}{gs.FaseNaar}]) && (PR[{_fcpf}{gs.FaseNaar}] || AR[{_fcpf}{gs.FaseNaar}]) && A[{_fcpf}{gs.FaseVan}] && R[{_fcpf}{gs.FaseVan}] && !TRG[{_fcpf}{gs.FaseVan}] && !kcv({_fcpf}{gs.FaseVan})));");
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schgs}{gs.FaseVan}{gs.FaseNaar}]) set_MRLW({_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, ({c.GetBoolV()}) ((RA[{_fcpf}{gs.FaseVan}] || SG[{_fcpf}{gs.FaseVan}]) && (PR[{_fcpf}{gs.FaseVan}] || AR[{_fcpf}{gs.FaseVan}]) && A[{_fcpf}{gs.FaseNaar}] && R[{_fcpf}{gs.FaseNaar}] && !TRG[{_fcpf}{gs.FaseNaar}] && !kcv({_fcpf}{gs.FaseNaar})));");
                        }
                    }
                    return sb.ToString();

				case CCOLCodeTypeEnum.PrioCIncludes:
					sb.AppendLine("#include \"syncvar.h\"");
					return sb.ToString();

				default:
                    return null;
            }
        }

        public override bool HasSettings() => true;

        #region Constructor
        #endregion // Constructor
    }
}