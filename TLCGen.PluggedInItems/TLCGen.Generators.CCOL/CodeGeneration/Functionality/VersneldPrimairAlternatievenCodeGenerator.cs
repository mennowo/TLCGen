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
    public class VersneldPrimairAlternatievenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmmlfpr;
        private CCOLGeneratorCodeStringSettingModel _prmaltg;
        private CCOLGeneratorCodeStringSettingModel _prmaltp;
        private CCOLGeneratorCodeStringSettingModel _schaltg;
#pragma warning restore 0649
        private string _tnlsg;
        private string _tnlfg;
        private string _tnlcv;
        private string _tnleg;
        private string _tnlsgd;
        private string _tnlfgd;
        private string _tnlcvd;
        private string _tnlegd;
	    private string _hfile;
	    private string _hmlact;
	    private string _hplact;
	    private string _hnla;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
            foreach (var fc in c.ModuleMolen.FasenModuleData)
            {
                // Vooruit realisaties
                _myElements.Add(
                    new CCOLElement(
                        $"{_prmmlfpr}{fc.FaseCyclus}",
                        fc.ModulenVooruit,
                        CCOLElementTimeTypeEnum.None,
                        CCOLElementTypeEnum.Parameter));
            }



            // Alternatieven
            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                foreach (var fc in c.ModuleMolen.FasenModuleData)
                {
                    _myElements.Add(
                        new CCOLElement(
                            $"{_prmaltg}{fc.FaseCyclus}",
                            fc.AlternatieveGroenTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Parameter));

                    // For prmaltp and schaltg: combine if the signal group is part of a simultaneous synchronisation
                    var gs = gelijkstarttuples.FirstOrDefault(x => x.Item1 == fc.FaseCyclus);

                    if (gs != null)
                    {
                        var namealtp = _prmaltp + string.Join(string.Empty, gs.Item2);
                        var namealtg = _schaltg + string.Join(string.Empty, gs.Item2);
                        var containsaltp = _myElements.Any(i => i.Naam == namealtp);
                        var containsaltg = _myElements.Any(i => i.Naam == namealtg);
                        if (!containsaltp)
                        {
                            _myElements.Add(
                                new CCOLElement(
                                    namealtp,
                                    fc.AlternatieveRuimte,
                                    CCOLElementTimeTypeEnum.TE_type,
                                    CCOLElementTypeEnum.Parameter));
                        }
                        if (!containsaltg)
                        {
                            _myElements.Add(
                                new CCOLElement(
                                    namealtg,
                                    fc.AlternatiefToestaan ? 1 : 0,
                                    CCOLElementTimeTypeEnum.SCH_type,
                                    CCOLElementTypeEnum.Schakelaar));
                        }
                    }
                    else
                    {
                        _myElements.Add(
                            new CCOLElement(
                                $"{_prmaltp}{fc.FaseCyclus}",
                                fc.AlternatieveRuimte,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Parameter));
                        _myElements.Add(
                            new CCOLElement(
                                $"{_schaltg}{fc.FaseCyclus}",
                                fc.AlternatiefToestaan ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                CCOLElementTypeEnum.Schakelaar));
                    }
                }
            }
            else
            {
                // Build a list of dictionaries for signalgroups that may realize alternatively;
                // each dictionary contains alternative signalgroups as keys, and a list of primary
                // signalgroups under whose CV the alternative may realise
                var altsdict =
                    new List<Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>>();
                foreach (var ml in c.ModuleMolen.Modules)
                {
                    var altdict =
                        new Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>();
                    foreach (var mlfc in ml.Fasen)
                    {
                        foreach (var amlfc in mlfc.Alternatieven)
                        {
                            if (!altdict.ContainsKey(amlfc.FaseCyclus))
                            {
                                altdict.Add(
                                    amlfc.FaseCyclus, 
                                    new List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>
                                    {
                                        new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc, mlfc)
                                    });
                            }
                        }
                    }
                    altsdict.Add(altdict);
                }
                var mlidx = 1;
                foreach (var alts in altsdict)
                {
                    foreach (var altg in alts)
                    {
                        _myElements.Add(
                            new CCOLElement(
                                $"{_prmaltg}{mlidx}{altg.Key}",
                                altg.Value.First().Item1.AlternatieveGroenTijd,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Parameter));
                    }
                    ++mlidx;
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules:
                    return 10;
                case CCOLCodeTypeEnum.HstCAlternatief:
                    return 10;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules:
                    sb.AppendLine($"{ts}/* versnelde primaire realisaties */");
                    sb.AppendLine($"{ts}/* ------------------------------ */");
                    sb.AppendLine($"{ts}/* periode versnelde primaire realisatie - aantal modulen vooruit */");
                    sb.AppendLine($"{ts}/* -------------------------------------------------------------- */");
                    foreach (var fc in c.ModuleMolen.FasenModuleData)
                        sb.AppendLine($"    PFPR[{_fcpf}{fc.FaseCyclus}] = ml_fpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmlfpr}{fc.FaseCyclus}], PRML, ML, MLMAX);");
                    sb.AppendLine("");
                    sb.AppendLine($"{ts}VersneldPrimair_Add();");
                    sb.AppendLine("");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}set_FPRML(fc, PRML, ML, MLMAX, (bool)PFPR[fc]);");
                    sb.AppendLine();
                    if (c.ModuleMolen.LangstWachtendeAlternatief)
                    {
                        sb.AppendLine($"{ts}/* langstwachtende alternatieve realisatie */");
                        sb.AppendLine($"{ts}/* --------------------------------------- */");
                        sb.AppendLine("");
                        sb.AppendLine($"{ts}afsluiten_aanvraaggebied_pr(PRML, ML);");
                        sb.AppendLine("");
                        sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
                        sb.AppendLine($"{ts}" + "{");
                        sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}{ts}FM[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}" + "}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* zet richtingen die alternatief gaan realiseren         */");
                        sb.AppendLine($"{ts}/* terug naar RV als er geen alternatieve ruimte meer is. */");
                        if (c.Fasen.Any(x => x.WachttijdVoorspeller))
                        {
                            sb.AppendLine($"{ts}/* Dit gebeurt niet voor fasen met een wachttijd voorspeller, */");
                            sb.AppendLine($"{ts}/* of fasen waarvan de voedende richting die heeft. */");
                        }
                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            // find signalgroup instance
                            var ffc = c.Fasen.FirstOrDefault(x => x.Naam == fc.FaseCyclus);
                            // if the sg has no predictor
                            if (ffc != null && !ffc.WachttijdVoorspeller)
                            {
                                // find a potential feeding sg
                                var fcnl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseNaar == fc.FaseCyclus);
                                // if there is a feeding sg, set the sg instance to that sg
                                if (fcnl != null) ffc = c.Fasen.FirstOrDefault(x => x.Naam == fcnl.FaseVan);
                            }
                            // if the instance is not null, and it has a predictor, skip setting RR
                            if (ffc != null && ffc.WachttijdVoorspeller) continue;
                            sb.AppendLine(
                                $"{ts}RR[{_fcpf}{fc.FaseCyclus}] |= R[{_fcpf}{fc.FaseCyclus}] && AR[{_fcpf}{fc.FaseCyclus}] && (!PAR[{_fcpf}{fc.FaseCyclus}] || ERA[{_fcpf}{fc.FaseCyclus}]) ? BIT5 : 0;");
                        }
                        sb.AppendLine();

                        var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                        var yes = false;
                        foreach (var gs in gelijkstarttuples)
                        {
                            if (gs.Item2.Count > 1)
                            {
                                yes = true;
                                sb.Append($"{ts}RR[{_fcpf}{gs.Item1}] |= R[{_fcpf}{gs.Item1}] && ");
                                if (gs.Item2.Count > 1) sb.Append("(");
                                var i = 0;
                                foreach (var ofc in gs.Item2)
                                {
                                    if (ofc == gs.Item1)
                                    {
                                        continue;
                                    }

                                    if (i > 0) sb.Append(" || ");
                                    sb.Append($"(RR[{_fcpf}{ofc}] & BIT5)");
                                    ++i;
                                }
                                if (gs.Item2.Count > 1) sb.Append(")");
                                sb.AppendLine(" ? BIT5 : 0;");
                            }
                        }
                        if (yes)
                        {
                            sb.AppendLine();
                        }

                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            sb.AppendLine(
                                $"{ts}FM[{_fcpf}{fc.FaseCyclus}] |= (fm_ar_kpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltg}{fc.FaseCyclus}])) ? BIT5 : 0;");
                        }
                        sb.AppendLine();

                        var maxtartotig = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "max_tar_tig" : "max_tar_to";

                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            Tuple<string, List<string>> hasgs = null;
                            foreach (var gs in gelijkstarttuples)
                            {
                                if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                                {
                                    hasgs = gs;
                                    break;
                                }
                            }
                            if (hasgs != null)
                            {
                                sb.Append(
                                    $"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = ({maxtartotig}({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}");
                                foreach (var ofc in hasgs.Item2)
                                {
                                    sb.Append(ofc);
                                }
                                sb.Append($"]) && SCH[{_schpf}{_schaltg}");
                                foreach (var ofc in hasgs.Item2)
                                {
                                    sb.Append(ofc);
                                }
                                sb.AppendLine("];");
                            }
                            else
                            {
                                sb.AppendLine(
                                    $"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = ({maxtartotig}({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}]) && SCH[{_schpf}{_schaltg}{fc.FaseCyclus}];");
                            }
                        }
                        sb.AppendLine();

                        if (c.InterSignaalGroep.Nalopen.Count > 0)
                        {
                            #region Get naloop type timer
                            var tnl = "";
                            if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroenDetectie))
                            {
                                tnl = _tnlfgd;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.StartGroenDetectie))
                            {
                                tnl = _tnlsgd;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeGroenDetectie))
                            {
                                tnl = _tnlegd;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroenDetectie))
                            {
                                tnl = _tnlcvd;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroen))
                            {
                                tnl = _tnlfg;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.StartGroen))
                            {
                                tnl = _tnlsg;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeGroen))
                            {
                                tnl = _tnleg;
                            }
                            else if (nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroen))
                            {
                                tnl = _tnlcv;
                            }
                            #endregion
                            sb.AppendLine($"{ts}/* Verzorgen PAR voor voedende richtingen */");
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
                                sb.AppendLine(
                                    $"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && (({maxtartotig}({_fcpf}{nl.FaseNaar}) >= T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}]) || G[{_fcpf}{nl.FaseVan}] || !A[{_fcpf}{nl.FaseNaar}]);");
                            }
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* Verzorgen PAR voor naloop richtingen */");
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
                                sb.AppendLine(
                                    $"{ts}PAR[{_fcpf}{nl.FaseNaar}] = PAR[{_fcpf}{nl.FaseNaar}] || RA[{_fcpf}{nl.FaseNaar}] || FG[{_fcpf}{nl.FaseNaar}]);");
                            }
                            sb.AppendLine();
                        }

                        yes = false;
                        foreach (var gs in gelijkstarttuples)
                        {
                            if (gs.Item2.Count > 1)
                            {
                                yes = true;
                                sb.Append($"{ts}PAR[{_fcpf}{gs.Item1}] = PAR[{_fcpf}{gs.Item1}]");
                                foreach (var ofc in gs.Item2)
                                {
                                    if (ofc == gs.Item1)
                                    {
                                        continue;
                                    }
                                    sb.Append($" && (PAR[{_fcpf}{ofc}] || !A[{_fcpf}{ofc}])");
                                }
                                sb.AppendLine(";");
                            }
                        }
                        if (yes)
                        {
                            sb.AppendLine();
                        }

                        yes = false;
                        foreach (var fcm in c.Fasen)
                        {
                            if (fcm.Meeverlengen != NooitAltijdAanUitEnum.Nooit)
                            {
                                {
                                    var fm = c.FileIngrepen.FirstOrDefault(
                                        x => x.TeDoserenSignaalGroepen.Any(x2 => x2.FaseCyclus == fcm.Naam));
                                    if (fm != null)
                                    {
                                        if (!yes)
                                        {
                                            yes = true;
                                            sb.AppendLine();
                                            sb.AppendLine($"{ts}/* Niet alternatief komen tijdens file */");
                                        }
                                        sb.AppendLine(
                                            $"{ts}if (IH[{_hpf}{_hfile}{fm.Naam}]) PAR[{_fcpf}{fcm.Naam}] = FALSE;");
                                    }
                                }
                            }
                        }
                        if (yes)
                        {
                            sb.AppendLine();
                        }

                        foreach (var gen in CCOLGenerator.OrderedPieceGenerators[CCOLCodeTypeEnum.RegCAlternatieven])
                        {
                            sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.RegCAlternatieven, ts));
                            sb.AppendLine();
                        }

                        sb.AppendLine($"{ts}Alternatief_Add();");
	                    if (c.HalfstarData.IsHalfstar)
	                    {
		                    sb.AppendLine($"{ts}Alternatief_halfstar();");
	                    }
                        sb.AppendLine();
                        sb.AppendLine($"{ts}langstwachtende_alternatief_modulen(PRML, ML, ML_MAX);");
                    }
                    else
                    {
                        if (c.ModuleMolen.Modules.SelectMany(x => x.Fasen).Any(x => x.Alternatieven.Any()))
                        {
                            sb.AppendLine($"{ts}/* alternatieve realisaties */");
                            sb.AppendLine($"{ts}/* ------------------------ */");

                            // Build a list of dictionaries for signalgroups that may realize alternatively;
                            // each dictionary contains alternative signalgroups as keys, and a list of primary
                            // signalgroups under whose CV the alternative may realise
                            var modulesWithAlternatives =
                                new List<Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>>();
                            foreach (var ml in c.ModuleMolen.Modules)
                            {
                                var altdict =
                                    new Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>();
                                foreach (var mlfc in ml.Fasen)
                                {
                                    foreach (var amlfc in mlfc.Alternatieven)
                                    {
                                        if (!altdict.ContainsKey(amlfc.FaseCyclus))
                                        {
                                            altdict.Add(
                                                amlfc.FaseCyclus,
                                                new List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>
                                                {
                                                    new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc,
                                                        mlfc)
                                                });
                                        }
                                        else
                                        {
                                            altdict[amlfc.FaseCyclus].Add(new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc, mlfc));
                                        }
                                    }
                                }
                                modulesWithAlternatives.Add(altdict);
                            }

                            var mlidx = 1;
                            foreach (var moduleWithAlternatives in modulesWithAlternatives)
                            {
                                foreach (var alternativeSignalGroup in moduleWithAlternatives)
                                {
                                    var pre = $"{ts}set_FARML({_fcpf}{alternativeSignalGroup.Key}, PRML, ML, ML_MAX, ML{mlidx}, "
                                        .Length;
                                    sb.Append(
                                        $"{ts}set_FARML({_fcpf}{alternativeSignalGroup.Key}, PRML, ML, ML_MAX, ML{mlidx}, ");
                                    var i = 0;
                                    foreach (var primarySignalGroup in alternativeSignalGroup.Value)
                                    {
                                        if (i > 0)
                                        {
                                            sb.AppendLine(" || ");
                                            sb.Append("".PadLeft(pre));
                                        }
                                        ++i;
                                        sb.Append($"(bool) (CV[{_fcpf}{primarySignalGroup.Item2.FaseCyclus}] && AlternatieveRuimte({_fcpf}{alternativeSignalGroup.Key}, {_fcpf}{primarySignalGroup.Item2.FaseCyclus}, {_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}))");
                                    }
                                    sb.AppendLine(");");
                                }
                                ++mlidx;
                            }
                            sb.AppendLine();
                            sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT5;");
                            sb.AppendLine($"{ts}{ts}FM[fc] &= ~BIT5;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine();
                            mlidx = 1;
                            foreach (var moduleWithAlternatives in modulesWithAlternatives)
                            {
                                foreach (var alternativeSignalGroup in moduleWithAlternatives)
                                {

                                    var pre = $"{ts}RR[{_fcpf}{alternativeSignalGroup.Key}] |= (AR[{_fcpf}{alternativeSignalGroup.Key}] && RA[{_fcpf}{alternativeSignalGroup.Key}] && "
                                        .Length;
                                    sb.Append(
                                        $"{ts}RR[{_fcpf}{alternativeSignalGroup.Key}] |= (AR[{_fcpf}{alternativeSignalGroup.Key}] && RA[{_fcpf}{alternativeSignalGroup.Key}] && ");
                                    var i = 0;
                                    foreach (var primarySignalGroup in alternativeSignalGroup.Value)
                                    {
                                        if (i > 0)
                                        {
                                            sb.AppendLine(" && ");
                                            sb.Append("".PadLeft(pre));
                                        }
                                        ++i;
                                        sb.Append($"!(CV[{_fcpf}{primarySignalGroup.Item2.FaseCyclus}] && AlternatieveRuimte({_fcpf}{alternativeSignalGroup.Key}, " +
                                                  $"{_fcpf}{primarySignalGroup.Item2.FaseCyclus}, {_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}))");
                                    }
                                    sb.AppendLine(") ? BIT5 : 0;");
                                }
                                ++mlidx;
                            }
                            sb.AppendLine();
                            mlidx = 1;
                            foreach (var moduleWithAlternatives in modulesWithAlternatives)
                            {
                                foreach (var alternativeSignalGroup in moduleWithAlternatives)
                                {
                                    sb.AppendLine(
                                        $"{ts}FM[{_fcpf}{alternativeSignalGroup.Key}] |= (AR[{_fcpf}{alternativeSignalGroup.Key}] && " +
                                        $"G[{_fcpf}{alternativeSignalGroup.Key}] && fm_ar_kpr({_fcpf}{alternativeSignalGroup.Key}, PRM[{_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}])) ? BIT5 : 0;");
                                }
                                ++mlidx;
                            }
                        }
                        sb.AppendLine();
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc) if (SR[fc] && AG[fc])");
                        sb.AppendLine($"{ts}{ts}if (SR[fc] && AG[fc])");
                        sb.AppendLine($"{ts}{ts}{ts}reset_FARML(fc, PRML, ML, ML_MAX);");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}Alternatief_Add();");
	                    if (c.HalfstarData.IsHalfstar)
	                    {
		                    sb.AppendLine($"{ts}Alternatief_halfstar();");
	                    }
                    }
                    sb.AppendLine();

                    return sb.ToString();
                case CCOLCodeTypeEnum.HstCAlternatief:
                    var gelijkstarttuples2 = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                    foreach (var fc in c.ModuleMolen.FasenModuleData)
                    {
                        Tuple<string, List<string>> hasgs = null;
                        foreach (var gs in gelijkstarttuples2)
                        {
                            if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                            {
                                hasgs = gs;
                                break;
                            }
                        }
                        if (hasgs != null)
                        {
                            sb.Append(
                                $"{ts}alternatief_halfstar({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltp}");
                            foreach (var ofc in hasgs.Item2)
                            {
                                sb.Append(ofc);
                            }
                            sb.Append($"], SCH[{_schpf}{_schaltg}");
                            foreach (var ofc in hasgs.Item2)
                            {
                                sb.Append(ofc);
                            }
                            sb.AppendLine("]);");
                        }
                        else
                        {
                            sb.AppendLine(
                                $"{ts}alternatief_halfstar({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}], SCH[{_schpf}{_schaltg}{fc.FaseCyclus}]);");
                        }
                    }
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        if (nl.Type == NaloopTypeEnum.EindeGroen ||
                            nl.Type == NaloopTypeEnum.CyclischVerlengGroen)
                        {
                            var t = nl.Type == NaloopTypeEnum.EindeGroen ? $"{_tpf}{_tnleg}{nl.FaseVan}{nl.FaseNaar}" : $"{_tpf}{_tnlcv}{nl.FaseVan}{nl.FaseNaar}";
                            if (nl.DetectieAfhankelijk)
                            {
                                t = nl.Type == NaloopTypeEnum.EindeGroen ? $"{_tpf}{_tnlegd}{nl.FaseVan}{nl.FaseNaar}" : $"{_tpf}{_tnlcvd}{nl.FaseVan}{nl.FaseNaar}";
                            }
                            sb.AppendLine($"{ts}altcor_kop_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {t});");
                        }
                        if (nl.Type == NaloopTypeEnum.StartGroen)
                        {
                            if (nl.DetectieAfhankelijk && nl.Detectoren.Any())
                            {
                                sb.AppendLine($"{ts}altcor_naloopSG_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, IH[{_hpf}{_hnla}{nl.Detectoren[0].Detector}], {_tpf}{_tnlsgd}{nl.FaseVan}{nl.FaseNaar}, TRUE);");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}altcor_naloopSG_halfstar({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, TRUE, {_tpf}{_tnlsg}{nl.FaseVan}{nl.FaseNaar}, TRUE);");
                            }
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
            _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
            _hmlact = CCOLGeneratorSettingsProvider.Default.GetElementName("hmlact");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _hnla = CCOLGeneratorSettingsProvider.Default.GetElementName("hnla");

            return base.SetSettings(settings);
        }
    }
}
